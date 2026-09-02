using Fluy.Domain.Workflows;

namespace Fluy.Application.Common.Interfaces.Repositories;

public interface IWorkflowInstanceRepository
{
    void Add(WorkflowInstance instance);
    Task<WorkflowInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<WorkflowInstance?> GetRunningForRequestAsync(Guid requestId, CancellationToken cancellationToken);
}
