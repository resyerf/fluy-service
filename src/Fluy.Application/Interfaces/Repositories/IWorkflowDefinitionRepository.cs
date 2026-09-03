using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IWorkflowDefinitionRepository
{
    Task<WorkflowDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(WorkflowDefinition definition);
    Task<IReadOnlyCollection<WorkflowDefinition>> GetOtherPublishedAsync(Guid tenantId, Guid excludeId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<WorkflowDefinitionSummary>> GetAllWithVersionsAsync(CancellationToken cancellationToken);
}
