using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Workflows;

/// <summary>
/// Motor de Workflow genérico (CLAUDE.md §14-16, CODE.md §9.20) — reemplaza el escalamiento
/// hardcodeado de <see cref="Fluy.Domain.Rules.ApprovalRule"/> (CODE.md §9.19, ahora superseded):
/// una definición agrupa versiones (<see cref="WorkflowVersion"/>), cada una con su propio grafo de
/// pasos/transiciones. Solo una definición puede estar Published por tenant a la vez (mismo alcance
/// mínimo de "un solo flujo activo" que tenía ApprovalRule) — soportar varios procesos concurrentes
/// por tenant queda para cuando Request tenga un selector de tipo de proceso (CLAUDE.md §1, fuera de
/// este alcance).
/// </summary>
public class WorkflowDefinition : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public WorkflowDefinitionStatus Status { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private WorkflowDefinition()
    {
    }

    public static WorkflowDefinition Create(Guid tenantId, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del workflow es obligatorio.", nameof(name));
        }

        return new WorkflowDefinition
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Status = WorkflowDefinitionStatus.Draft
        };
    }

    public void MarkPublished()
    {
        if (Status == WorkflowDefinitionStatus.Archived)
        {
            throw new InvalidOperationException("No se puede publicar un workflow archivado.");
        }

        Status = WorkflowDefinitionStatus.Published;
    }

    public void Archive()
    {
        Status = WorkflowDefinitionStatus.Archived;
    }
}
