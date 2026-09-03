using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Queries.Approvals.GetPendingApprovals;

/// <summary>
/// "Pendientes" del dashboard del Aprobador (CLAUDE.md §24). Excluye los Approval de tier 2 cuyo
/// `RequiredRoleId` (CODE.md §9.19) el usuario actual no tiene — mostrarlos igual llevaría a un 403
/// al intentar decidirlos, ya que `ApprovalAuthorizationService` los bloquea del lado del Command.
/// </summary>
public class GetPendingApprovalsQueryHandler(IApprovalRepository approvals, ICurrentUserService currentUser)
    : IQueryHandler<GetPendingApprovalsQuery, IReadOnlyCollection<PendingApprovalSummary>>
{
    public Task<IReadOnlyCollection<PendingApprovalSummary>> Handle(
        GetPendingApprovalsQuery query, CancellationToken cancellationToken) =>
        approvals.GetPendingForUserAsync(currentUser.UserId!.Value, query.BranchId, cancellationToken);
}
