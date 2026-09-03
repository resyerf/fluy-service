using Fluy.Domain.Common;
using Fluy.SharedKernel;
using Fluy.Domain.Enums;

namespace Fluy.Domain.Entities;

/// <summary>
/// CLAUDE.md §27: publicar congela la versión — las instancias en curso quedan atadas a la versión
/// con la que iniciaron (<see cref="WorkflowInstance.WorkflowVersionId"/>) y nunca migran. Solo se
/// puede editar (agregar pasos/transiciones) mientras está en Draft.
/// </summary>
public class WorkflowVersion : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid WorkflowDefinitionId { get; private set; }
    public int VersionNumber { get; private set; }
    public WorkflowVersionStatus Status { get; private set; }
    public Guid? InitialStepId { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private WorkflowVersion()
    {
    }

    public static WorkflowVersion CreateDraft(Guid tenantId, Guid workflowDefinitionId, int versionNumber) => new()
    {
        TenantId = tenantId,
        WorkflowDefinitionId = workflowDefinitionId,
        VersionNumber = versionNumber,
        Status = WorkflowVersionStatus.Draft
    };

    public void SetInitialStep(Guid stepId)
    {
        EnsureDraft();
        InitialStepId = stepId;
    }

    /// <summary>
    /// Congela la versión. La validación del grafo se hace sobre los datos que el handler ya cargó
    /// (pasos/transiciones de esta versión) — el agregado no golpea la base de datos:
    /// - debe existir al menos un paso y un paso inicial válido.
    /// - todo paso debe tener al menos una transición de salida (<c>ToStepId == null</c> significa
    ///   "completar la solicitud", CODE.md §9.20).
    /// - si un paso tiene más de una transición, exactamente una debe quedar sin condición (la
    ///   transición por defecto) para garantizar que siempre hay un camino en tiempo de ejecución.
    /// </summary>
    public void Publish(IReadOnlyCollection<WorkflowStep> steps, IReadOnlyCollection<WorkflowTransition> transitions, DateTimeOffset now)
    {
        EnsureDraft();

        if (steps.Count == 0)
        {
            throw new InvalidOperationException("El workflow necesita al menos un paso antes de publicarse.");
        }

        if (InitialStepId is null || steps.All(s => s.Id != InitialStepId))
        {
            throw new InvalidOperationException("El workflow necesita un paso inicial válido antes de publicarse.");
        }

        foreach (var step in steps)
        {
            var outgoing = transitions.Where(t => t.FromStepId == step.Id).ToList();
            if (outgoing.Count == 0)
            {
                throw new InvalidOperationException($"El paso '{step.Name}' no tiene ninguna transición de salida.");
            }

            if (outgoing.Count > 1 && outgoing.Count(t => t.ConditionField is null) != 1)
            {
                throw new InvalidOperationException(
                    $"El paso '{step.Name}' tiene varias transiciones: exactamente una debe quedar sin condición (transición por defecto).");
            }
        }

        Status = WorkflowVersionStatus.Active;
        PublishedAt = now;
    }

    public void Archive() => Status = WorkflowVersionStatus.Archived;

    private void EnsureDraft()
    {
        if (Status != WorkflowVersionStatus.Draft)
        {
            throw new InvalidOperationException($"Esta versión ya no está en Draft (estado actual: {Status}).");
        }
    }
}
