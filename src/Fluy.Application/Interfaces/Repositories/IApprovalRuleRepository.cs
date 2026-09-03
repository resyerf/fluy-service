using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IApprovalRuleRepository
{
    Task<ApprovalRule?> GetForTenantAsync(Guid tenantId, CancellationToken cancellationToken);
    void Add(ApprovalRule rule);
    Task<ApprovalRuleDetail?> GetDetailAsync(CancellationToken cancellationToken);
}
