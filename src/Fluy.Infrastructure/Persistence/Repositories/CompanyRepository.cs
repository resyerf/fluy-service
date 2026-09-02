using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Application.Organization.GetCompanies;
using Fluy.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class CompanyRepository(ApplicationDbContext db) : ICompanyRepository
{
    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) =>
        db.Companies.AnyAsync(c => c.Id == id, cancellationToken);

    public void Add(Company company) => db.Companies.Add(company);

    public async Task<IReadOnlyCollection<CompanyDetail>> GetAllAsync(CancellationToken cancellationToken) =>
        await db.Companies.AsNoTracking()
            .Select(c => new CompanyDetail(c.Id, c.Name, c.LegalIdentifier))
            .ToListAsync(cancellationToken);
}
