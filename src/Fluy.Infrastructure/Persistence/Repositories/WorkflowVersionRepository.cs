using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Application.Workflows.GetWorkflowVersionDetail;
using Fluy.Domain.Workflows;
using Microsoft.EntityFrameworkCore;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class WorkflowVersionRepository(ApplicationDbContext db) : IWorkflowVersionRepository
{
    public Task<WorkflowVersion?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.WorkflowVersions.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public void Add(WorkflowVersion version) => db.WorkflowVersions.Add(version);

    public Task<WorkflowVersion?> GetActivePublishedForTenantAsync(Guid tenantId, CancellationToken cancellationToken) =>
        db.WorkflowVersions
            .Where(v => v.TenantId == tenantId && v.Status == WorkflowVersionStatus.Active)
            .Where(v => db.WorkflowDefinitions.Any(d => d.Id == v.WorkflowDefinitionId && d.Status == WorkflowDefinitionStatus.Published))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<WorkflowVersion?> GetActiveForDefinitionAsync(Guid definitionId, CancellationToken cancellationToken) =>
        db.WorkflowVersions
            .FirstOrDefaultAsync(v => v.WorkflowDefinitionId == definitionId && v.Status == WorkflowVersionStatus.Active, cancellationToken);

    public async Task<WorkflowVersionDetail> GetDetailAsync(Guid versionId, CancellationToken cancellationToken)
    {
        var version = await db.WorkflowVersions.AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == versionId, cancellationToken)
            ?? throw new WorkflowVersionNotFoundException(versionId);

        var steps = await (
                from step in db.WorkflowSteps.AsNoTracking()
                where step.WorkflowVersionId == version.Id
                join role in db.Roles.AsNoTracking() on step.ApproverRoleId equals role.Id
                orderby step.Order
                select new WorkflowStepDetail(step.Id, step.Name, step.ApproverRoleId, role.Name, step.Order))
            .ToListAsync(cancellationToken);

        var transitions = await db.WorkflowTransitions.AsNoTracking()
            .Where(t => t.WorkflowVersionId == version.Id)
            .OrderBy(t => t.Order)
            .Select(t => new WorkflowTransitionDetail(
                t.Id, t.FromStepId, t.ToStepId,
                t.ConditionField, t.ConditionOperator == null ? null : t.ConditionOperator.ToString(), t.ConditionValue, t.Order))
            .ToListAsync(cancellationToken);

        return new WorkflowVersionDetail(
            version.Id, version.WorkflowDefinitionId, version.VersionNumber, version.Status.ToString(), version.InitialStepId,
            steps, transitions);
    }

    public Task<int> CountStepsAsync(Guid versionId, CancellationToken cancellationToken) =>
        db.WorkflowSteps.Where(s => s.WorkflowVersionId == versionId).CountAsync(cancellationToken);

    public Task<bool> StepExistsAsync(Guid stepId, Guid versionId, CancellationToken cancellationToken) =>
        db.WorkflowSteps.AnyAsync(s => s.Id == stepId && s.WorkflowVersionId == versionId, cancellationToken);

    public Task<WorkflowStep?> GetStepAsync(Guid stepId, CancellationToken cancellationToken) =>
        db.WorkflowSteps.FirstOrDefaultAsync(s => s.Id == stepId, cancellationToken);

    public void AddStep(WorkflowStep step) => db.WorkflowSteps.Add(step);

    public async Task<IReadOnlyCollection<WorkflowStep>> GetStepsAsync(Guid versionId, CancellationToken cancellationToken) =>
        await db.WorkflowSteps.Where(s => s.WorkflowVersionId == versionId).ToListAsync(cancellationToken);

    public void AddTransition(WorkflowTransition transition) => db.WorkflowTransitions.Add(transition);

    public async Task<IReadOnlyCollection<WorkflowTransition>> GetTransitionsAsync(Guid versionId, CancellationToken cancellationToken) =>
        await db.WorkflowTransitions.Where(t => t.WorkflowVersionId == versionId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<WorkflowTransition>> GetTransitionsFromStepAsync(
        Guid fromStepId, CancellationToken cancellationToken) =>
        await db.WorkflowTransitions
            .Where(t => t.FromStepId == fromStepId)
            .OrderBy(t => t.ConditionField == null) // condicionales primero, la transición por defecto (sin condición) al final
            .ThenBy(t => t.Order)
            .ToListAsync(cancellationToken);
}
