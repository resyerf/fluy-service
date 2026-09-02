using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Workflows;
using Microsoft.EntityFrameworkCore;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class WorkflowInstanceRepository(ApplicationDbContext db) : IWorkflowInstanceRepository
{
    public void Add(WorkflowInstance instance) => db.WorkflowInstances.Add(instance);

    public Task<WorkflowInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.WorkflowInstances.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public Task<WorkflowInstance?> GetRunningForRequestAsync(Guid requestId, CancellationToken cancellationToken) =>
        db.WorkflowInstances
            .Where(i => i.RequestId == requestId && i.Status == WorkflowInstanceStatus.Running)
            .FirstOrDefaultAsync(cancellationToken);
}
