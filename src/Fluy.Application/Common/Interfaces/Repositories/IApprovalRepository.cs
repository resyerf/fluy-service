using Fluy.Application.Approvals.GetPendingApprovals;
using Fluy.Application.Requests.GetRequestById;
using Fluy.Domain.Approvals;

namespace Fluy.Application.Common.Interfaces.Repositories;

public interface IApprovalRepository
{
    Task<Approval?> GetPendingByRequestIdAsync(Guid requestId, CancellationToken cancellationToken);
    void Add(Approval approval);
    Task<IReadOnlyCollection<PendingApprovalSummary>> GetPendingForUserAsync(Guid userId, Guid? branchId, CancellationToken cancellationToken);
    Task<LatestApprovalDetail?> GetLatestForRequestAsync(Guid requestId, CancellationToken cancellationToken);
}
