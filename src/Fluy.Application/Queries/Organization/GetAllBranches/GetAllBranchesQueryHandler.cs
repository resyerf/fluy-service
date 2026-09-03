using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Organization.GetAllBranches;

public class GetAllBranchesQueryHandler(IBranchRepository branches)
    : IQueryHandler<GetAllBranchesQuery, IReadOnlyCollection<AllBranchDetail>>
{
    public Task<IReadOnlyCollection<AllBranchDetail>> Handle(GetAllBranchesQuery query, CancellationToken cancellationToken) =>
        branches.GetAllWithCompanyAsync(cancellationToken);
}
