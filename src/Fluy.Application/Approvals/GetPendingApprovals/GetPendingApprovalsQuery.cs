using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Approvals.GetPendingApprovals;

/// <summary>BranchId nulo = sin filtrar por sede (CODE.md §9.25, mismo criterio que GetMyRequestsQuery).</summary>
public record GetPendingApprovalsQuery(Guid? BranchId = null) : IQuery<IReadOnlyCollection<PendingApprovalSummary>>, IRequiresPermission
{
    public string PermissionCode => "request.approve";
}

public record PendingApprovalSummary(
    Guid ApprovalId, Guid RequestId, string RequestTitle, decimal? Amount, string RequesterEmail,
    DateTimeOffset? SubmittedAt, int Tier, string? RequiredRoleName);
