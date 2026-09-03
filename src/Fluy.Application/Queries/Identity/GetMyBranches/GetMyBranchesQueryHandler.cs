using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Queries.Identity.GetMyBranches;

public class GetMyBranchesQueryHandler(IUserRoleRepository userRoles, IBranchRepository branches, ICurrentUserService currentUser)
    : IQueryHandler<GetMyBranchesQuery, IReadOnlyCollection<MyBranchSummary>>
{
    public async Task<IReadOnlyCollection<MyBranchSummary>> Handle(GetMyBranchesQuery query, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var scopedBranchIds = await userRoles.GetBranchIdsForUserAsync(userId, cancellationToken);

        var hasTenantWideRole = scopedBranchIds.Any(branchId => branchId == null);
        if (hasTenantWideRole)
        {
            return await branches.GetAllSummariesAsync(cancellationToken);
        }

        var branchIds = scopedBranchIds.Where(branchId => branchId.HasValue).Select(branchId => branchId!.Value).Distinct().ToList();
        return await branches.GetSummariesByIdsAsync(branchIds, cancellationToken);
    }
}
