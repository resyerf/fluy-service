using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Application.Workflows.GetWorkflowDefinitions;
using Fluy.Domain.Workflows;
using Microsoft.EntityFrameworkCore;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class WorkflowDefinitionRepository(ApplicationDbContext db) : IWorkflowDefinitionRepository
{
    public Task<WorkflowDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.WorkflowDefinitions.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public void Add(WorkflowDefinition definition) => db.WorkflowDefinitions.Add(definition);

    public async Task<IReadOnlyCollection<WorkflowDefinition>> GetOtherPublishedAsync(
        Guid tenantId, Guid excludeId, CancellationToken cancellationToken) =>
        await db.WorkflowDefinitions
            .Where(d => d.TenantId == tenantId && d.Id != excludeId && d.Status == WorkflowDefinitionStatus.Published)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<WorkflowDefinitionSummary>> GetAllWithVersionsAsync(CancellationToken cancellationToken)
    {
        var definitions = await db.WorkflowDefinitions.AsNoTracking()
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);

        var versions = await db.WorkflowVersions.AsNoTracking().ToListAsync(cancellationToken);

        return definitions
            .Select(d => new WorkflowDefinitionSummary(
                d.Id, d.Name, d.Description, d.Status.ToString(),
                versions
                    .Where(v => v.WorkflowDefinitionId == d.Id)
                    .OrderBy(v => v.VersionNumber)
                    .Select(v => new WorkflowVersionSummary(v.Id, v.VersionNumber, v.Status.ToString()))
                    .ToList()))
            .ToList();
    }
}
