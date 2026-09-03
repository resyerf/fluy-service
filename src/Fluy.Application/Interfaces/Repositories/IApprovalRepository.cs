using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IApprovalRepository
{
    Task<Approval?> GetPendingByRequestIdAsync(Guid requestId, CancellationToken cancellationToken);
    void Add(Approval approval);
    Task<IReadOnlyCollection<PendingApprovalSummary>> GetPendingForUserAsync(Guid userId, Guid? branchId, CancellationToken cancellationToken);
    Task<LatestApprovalDetail?> GetLatestForRequestAsync(Guid requestId, CancellationToken cancellationToken);
}
