using Fluy.Application.Approvals.GetPendingApprovals;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Application.Requests.GetRequestById;
using Fluy.Domain.Approvals;
using Microsoft.EntityFrameworkCore;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class ApprovalRepository(ApplicationDbContext db) : IApprovalRepository
{
    public Task<Approval?> GetPendingByRequestIdAsync(Guid requestId, CancellationToken cancellationToken) =>
        db.Approvals.FirstOrDefaultAsync(a => a.RequestId == requestId && a.Status == ApprovalStatus.Pending, cancellationToken);

    public void Add(Approval approval) => db.Approvals.Add(approval);

    public async Task<IReadOnlyCollection<PendingApprovalSummary>> GetPendingForUserAsync(
        Guid userId, Guid? branchId, CancellationToken cancellationToken) =>
        await (
                from approval in db.Approvals
                where approval.Status == ApprovalStatus.Pending
                where approval.RequiredRoleId == null
                    || db.UserRoles.Any(ur => ur.UserId == userId && ur.RoleId == approval.RequiredRoleId)
                join request in db.Requests on approval.RequestId equals request.Id
                where branchId == null || request.BranchId == branchId
                join requester in db.Users on request.RequesterId equals requester.Id
                orderby request.SubmittedAt
                select new PendingApprovalSummary(
                    approval.Id, request.Id, request.Title, request.Amount, requester.Email, request.SubmittedAt,
                    approval.Tier,
                    approval.RequiredRoleId == null
                        ? null
                        : db.Roles.Where(r => r.Id == approval.RequiredRoleId).Select(r => r.Name).FirstOrDefault()))
            .ToListAsync(cancellationToken);

    public async Task<LatestApprovalDetail?> GetLatestForRequestAsync(Guid requestId, CancellationToken cancellationToken)
    {
        var latestApproval = await (
                from approval in db.Approvals.AsNoTracking()
                where approval.RequestId == requestId
                orderby approval.CreatedAt descending
                select new
                {
                    approval.Status,
                    approval.Comment,
                    approval.DecidedAt,
                    approval.Tier,
                    RequiredRoleName = approval.RequiredRoleId == null
                        ? null
                        : db.Roles.Where(r => r.Id == approval.RequiredRoleId).Select(r => r.Name).FirstOrDefault()
                })
            .FirstOrDefaultAsync(cancellationToken);

        return latestApproval is null
            ? null
            : new LatestApprovalDetail(
                latestApproval.Status.ToString(), latestApproval.Comment, latestApproval.DecidedAt,
                latestApproval.Tier, latestApproval.RequiredRoleName);
    }
}
