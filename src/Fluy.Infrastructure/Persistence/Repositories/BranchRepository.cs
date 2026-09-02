using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Application.Identity.GetMyBranches;
using Fluy.Application.Organization.GetAllBranches;
using Fluy.Application.Organization.GetBranchesByCompany;
using Fluy.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class BranchRepository(ApplicationDbContext db) : IBranchRepository
{
    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) =>
        db.Branches.AnyAsync(b => b.Id == id, cancellationToken);

    public void Add(Branch branch) => db.Branches.Add(branch);

    public async Task<IReadOnlyCollection<AllBranchDetail>> GetAllWithCompanyAsync(CancellationToken cancellationToken) =>
        await (
                from branch in db.Branches.AsNoTracking()
                join company in db.Companies.AsNoTracking() on branch.CompanyId equals company.Id
                orderby company.Name, branch.Name
                select new AllBranchDetail(branch.Id, branch.Name, company.Name))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<BranchDetail>> GetByCompanyAsync(Guid companyId, CancellationToken cancellationToken) =>
        await db.Branches.AsNoTracking()
            .Where(b => b.CompanyId == companyId)
            .Select(b => new BranchDetail(b.Id, b.Name))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<MyBranchSummary>> GetAllSummariesAsync(CancellationToken cancellationToken) =>
        await db.Branches.AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new MyBranchSummary(b.Id, b.Name))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<MyBranchSummary>> GetSummariesByIdsAsync(IReadOnlyCollection<Guid> branchIds, CancellationToken cancellationToken) =>
        await db.Branches.AsNoTracking()
            .Where(b => branchIds.Contains(b.Id))
            .OrderBy(b => b.Name)
            .Select(b => new MyBranchSummary(b.Id, b.Name))
            .ToListAsync(cancellationToken);
}
