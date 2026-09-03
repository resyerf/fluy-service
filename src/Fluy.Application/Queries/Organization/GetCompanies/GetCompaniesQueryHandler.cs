using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Organization.GetCompanies;

public class GetCompaniesQueryHandler(ICompanyRepository companies) : IQueryHandler<GetCompaniesQuery, IReadOnlyCollection<CompanyDetail>>
{
    public Task<IReadOnlyCollection<CompanyDetail>> Handle(GetCompaniesQuery query, CancellationToken cancellationToken) =>
        companies.GetAllAsync(cancellationToken);
}
