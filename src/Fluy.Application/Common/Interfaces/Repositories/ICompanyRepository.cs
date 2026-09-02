using Fluy.Application.Organization.GetCompanies;
using Fluy.Domain.Tenancy;

namespace Fluy.Application.Common.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    void Add(Company company);
    Task<IReadOnlyCollection<CompanyDetail>> GetAllAsync(CancellationToken cancellationToken);
}
