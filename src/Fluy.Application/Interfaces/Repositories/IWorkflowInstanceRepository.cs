using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IWorkflowInstanceRepository
{
    void Add(WorkflowInstance instance);
    Task<WorkflowInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<WorkflowInstance?> GetRunningForRequestAsync(Guid requestId, CancellationToken cancellationToken);
}
