using Fluy.Application.Interfaces.Repositories;
using Fluy.Application.DTOs;
using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Fluy.Infrastructure.Persistence.Context;

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
