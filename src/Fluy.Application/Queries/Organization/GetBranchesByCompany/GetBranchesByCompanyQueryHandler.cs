using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Organization.GetBranchesByCompany;

public class GetBranchesByCompanyQueryHandler(IBranchRepository branches)
    : IQueryHandler<GetBranchesByCompanyQuery, IReadOnlyCollection<BranchDetail>>
{
    public Task<IReadOnlyCollection<BranchDetail>> Handle(GetBranchesByCompanyQuery query, CancellationToken cancellationToken) =>
        branches.GetByCompanyAsync(query.CompanyId, cancellationToken);
}
