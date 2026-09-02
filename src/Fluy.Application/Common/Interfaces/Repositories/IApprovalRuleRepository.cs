using Fluy.Application.Rules.GetApprovalRule;
using Fluy.Domain.Rules;

namespace Fluy.Application.Common.Interfaces.Repositories;

public interface IApprovalRuleRepository
{
    Task<ApprovalRule?> GetForTenantAsync(Guid tenantId, CancellationToken cancellationToken);
    void Add(ApprovalRule rule);
    Task<ApprovalRuleDetail?> GetDetailAsync(CancellationToken cancellationToken);
}
