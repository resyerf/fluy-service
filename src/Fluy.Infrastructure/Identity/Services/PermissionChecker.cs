using Fluy.Application.Common.Exceptions;
using Fluy.Application.Interfaces.Services;
using Fluy.Infrastructure.Persistence.Context;
using Fluy.SharedKernel.Dispatching;
using Microsoft.EntityFrameworkCore;

namespace Fluy.Infrastructure.Identity.Services;

/// <summary>
/// Implementa TenantAuthorizationBehavior (CODE.md §4.4, cableado en §10-D19): resuelve
/// UserId → UserRole → RolePermission → Permission, filtrado automáticamente al tenant actual
/// por el Global Query Filter de EF Core — nunca se compara Permission.Code contra el catálogo
/// completo, solo contra lo que el usuario tiene efectivamente asignado.
/// </summary>
public class PermissionChecker(ApplicationDbContext db, ICurrentUserService currentUser) : IPermissionChecker
{
    public async Task EnsureAuthorizedAsync(string permissionCode, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new NotAuthorizedException(permissionCode);

        var hasPermission = await (
                from userRole in db.UserRoles
                join rolePermission in db.RolePermissions on userRole.RoleId equals rolePermission.RoleId
                join permission in db.Permissions on rolePermission.PermissionId equals permission.Id
                where userRole.UserId == userId && permission.Code == permissionCode
                select permission.Id)
            .AnyAsync(cancellationToken);

        if (!hasPermission)
        {
            throw new NotAuthorizedException(permissionCode);
        }
    }
}
