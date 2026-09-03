using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IWorkflowVersionRepository
{
    Task<WorkflowVersion?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(WorkflowVersion version);

    /// <summary>
    /// La versión Active de la definición Published del tenant (CODE.md §9.20) — nunca hay más de
    /// una a la vez (PublishWorkflowVersion archiva cualquier otra Active al publicar).
    /// </summary>
    Task<WorkflowVersion?> GetActivePublishedForTenantAsync(Guid tenantId, CancellationToken cancellationToken);

    Task<WorkflowVersion?> GetActiveForDefinitionAsync(Guid definitionId, CancellationToken cancellationToken);
    Task<WorkflowVersionDetail> GetDetailAsync(Guid versionId, CancellationToken cancellationToken);

    Task<int> CountStepsAsync(Guid versionId, CancellationToken cancellationToken);
    Task<bool> StepExistsAsync(Guid stepId, Guid versionId, CancellationToken cancellationToken);
    Task<WorkflowStep?> GetStepAsync(Guid stepId, CancellationToken cancellationToken);
    void AddStep(WorkflowStep step);
    Task<IReadOnlyCollection<WorkflowStep>> GetStepsAsync(Guid versionId, CancellationToken cancellationToken);

    void AddTransition(WorkflowTransition transition);
    Task<IReadOnlyCollection<WorkflowTransition>> GetTransitionsAsync(Guid versionId, CancellationToken cancellationToken);

    /// <summary>
    /// Ordenadas: transiciones condicionales primero, la transición por defecto (sin condición) al final.
    /// </summary>
    Task<IReadOnlyCollection<WorkflowTransition>> GetTransitionsFromStepAsync(Guid fromStepId, CancellationToken cancellationToken);
}
