using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    void Add(Company company);
    Task<IReadOnlyCollection<CompanyDetail>> GetAllAsync(CancellationToken cancellationToken);
}
