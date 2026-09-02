using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.GetAllBranches;

public class GetAllBranchesQueryHandler(IBranchRepository branches)
    : IQueryHandler<GetAllBranchesQuery, IReadOnlyCollection<AllBranchDetail>>
{
    public Task<IReadOnlyCollection<AllBranchDetail>> Handle(GetAllBranchesQuery query, CancellationToken cancellationToken) =>
        branches.GetAllWithCompanyAsync(cancellationToken);
}
