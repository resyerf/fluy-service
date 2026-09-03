using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Fluy.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Fluy.Infrastructure.Persistence.Context;

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
