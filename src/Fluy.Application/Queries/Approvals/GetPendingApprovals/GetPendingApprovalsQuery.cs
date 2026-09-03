using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Approvals.GetPendingApprovals;

/// <summary>BranchId nulo = sin filtrar por sede (CODE.md §9.25, mismo criterio que GetMyRequestsQuery).</summary>
public record GetPendingApprovalsQuery(Guid? BranchId = null) : IQuery<IReadOnlyCollection<PendingApprovalSummary>>, IRequiresPermission
{
    public string PermissionCode => "request.approve";
}
