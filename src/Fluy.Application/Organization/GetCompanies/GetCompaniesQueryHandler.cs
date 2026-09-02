using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.GetCompanies;

public class GetCompaniesQueryHandler(ICompanyRepository companies) : IQueryHandler<GetCompaniesQuery, IReadOnlyCollection<CompanyDetail>>
{
    public Task<IReadOnlyCollection<CompanyDetail>> Handle(GetCompaniesQuery query, CancellationToken cancellationToken) =>
        companies.GetAllAsync(cancellationToken);
}
