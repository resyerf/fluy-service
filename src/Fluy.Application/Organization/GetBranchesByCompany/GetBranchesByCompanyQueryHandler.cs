using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.GetBranchesByCompany;

public class GetBranchesByCompanyQueryHandler(IBranchRepository branches)
    : IQueryHandler<GetBranchesByCompanyQuery, IReadOnlyCollection<BranchDetail>>
{
    public Task<IReadOnlyCollection<BranchDetail>> Handle(GetBranchesByCompanyQuery query, CancellationToken cancellationToken) =>
        branches.GetByCompanyAsync(query.CompanyId, cancellationToken);
}
