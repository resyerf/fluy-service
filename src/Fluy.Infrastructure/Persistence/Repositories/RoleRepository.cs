using Fluy.Application.Interfaces.Repositories;
using Fluy.Application.DTOs;
using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Fluy.Infrastructure.Persistence.Context;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class RoleRepository(ApplicationDbContext db) : IRoleRepository
{
    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) =>
        db.Roles.AnyAsync(r => r.Id == id, cancellationToken);

    public void Add(Role role) => db.Roles.Add(role);

    public void AddPermissions(IEnumerable<RolePermission> rolePermissions) => db.RolePermissions.AddRange(rolePermissions);

    public async Task<IReadOnlyCollection<RoleDetail>> GetAllWithPermissionsAsync(CancellationToken cancellationToken)
    {
        var roles = await db.Roles.AsNoTracking()
            .Select(r => new { r.Id, r.Name, r.IsSystemRole })
            .ToListAsync(cancellationToken);

        var rolePermissions = await db.RolePermissions.AsNoTracking()
            .Join(db.Permissions.AsNoTracking(), rp => rp.PermissionId, p => p.Id, (rp, p) => new { rp.RoleId, p.Code })
            .ToListAsync(cancellationToken);

        return roles
            .Select(r => new RoleDetail(
                r.Id,
                r.Name,
                r.IsSystemRole,
                rolePermissions.Where(rp => rp.RoleId == r.Id).Select(rp => rp.Code).OrderBy(c => c).ToList()))
            .ToList();
    }

    public Task<string?> GetNameAsync(Guid roleId, CancellationToken cancellationToken) =>
        db.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefaultAsync(cancellationToken);
}
