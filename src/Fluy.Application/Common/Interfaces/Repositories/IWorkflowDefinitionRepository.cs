using Fluy.Application.Workflows.GetWorkflowDefinitions;
using Fluy.Domain.Workflows;

namespace Fluy.Application.Common.Interfaces.Repositories;

public interface IWorkflowDefinitionRepository
{
    Task<WorkflowDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(WorkflowDefinition definition);
    Task<IReadOnlyCollection<WorkflowDefinition>> GetOtherPublishedAsync(Guid tenantId, Guid excludeId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<WorkflowDefinitionSummary>> GetAllWithVersionsAsync(CancellationToken cancellationToken);
}
