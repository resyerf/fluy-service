using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Application.Rules.GetApprovalRule;
using Fluy.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class ApprovalRuleRepository(ApplicationDbContext db) : IApprovalRuleRepository
{
    public Task<ApprovalRule?> GetForTenantAsync(Guid tenantId, CancellationToken cancellationToken) =>
        db.ApprovalRules.FirstOrDefaultAsync(r => r.TenantId == tenantId, cancellationToken);

    public void Add(ApprovalRule rule) => db.ApprovalRules.Add(rule);

    public Task<ApprovalRuleDetail?> GetDetailAsync(CancellationToken cancellationToken) =>
        (
            from rule in db.ApprovalRules
            join role in db.Roles on rule.SecondApproverRoleId equals role.Id
            select new ApprovalRuleDetail(rule.Id, rule.MinAmount, rule.SecondApproverRoleId, role.Name))
        .FirstOrDefaultAsync(cancellationToken);
}
