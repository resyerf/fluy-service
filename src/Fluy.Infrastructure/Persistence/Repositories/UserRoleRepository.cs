using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Fluy.Infrastructure.Persistence.Context;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class UserRoleRepository(ApplicationDbContext db) : IUserRoleRepository
{
    public void Add(UserRole userRole) => db.UserRoles.Add(userRole);

    public async Task<IReadOnlyCollection<Guid?>> GetBranchIdsForUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await db.UserRoles.AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.BranchId)
            .ToListAsync(cancellationToken);

    public Task<bool> HasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken) =>
        db.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

    public async Task<IReadOnlyCollection<Guid>> GetUserIdsByRoleAsync(Guid roleId, CancellationToken cancellationToken) =>
        await db.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Guid>> GetUserIdsWithPermissionAsync(string permissionCode, CancellationToken cancellationToken) =>
        await (
                from userRole in db.UserRoles
                join rolePermission in db.RolePermissions on userRole.RoleId equals rolePermission.RoleId
                join permission in db.Permissions on rolePermission.PermissionId equals permission.Id
                where permission.Code == permissionCode
                select userRole.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);
}
