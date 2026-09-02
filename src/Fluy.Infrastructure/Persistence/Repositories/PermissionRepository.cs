using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Application.Identity.GetPermissionCatalog;
using Fluy.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class PermissionRepository(ApplicationDbContext db) : IPermissionRepository
{
    public async Task<IReadOnlyCollection<PermissionDetail>> GetCatalogAsync(CancellationToken cancellationToken) =>
        await db.Permissions.AsNoTracking()
            .OrderBy(p => p.Code)
            .Select(p => new PermissionDetail(p.Id, p.Code, p.Description))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Permission>> GetByCodesAsync(IReadOnlyCollection<string> codes, CancellationToken cancellationToken) =>
        await db.Permissions.Where(p => codes.Contains(p.Code)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Permission>> GetAllAsync(CancellationToken cancellationToken) =>
        await db.Permissions.ToListAsync(cancellationToken);
}
