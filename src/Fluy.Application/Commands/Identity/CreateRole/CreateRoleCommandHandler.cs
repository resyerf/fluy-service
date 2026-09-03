using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Identity.CreateRole;

/// <summary>
/// Crea un rol de tenant (siempre IsSystemRole = false: el único rol de sistema es TenantAdmin,
/// creado por BootstrapTenantCommandHandler) y le asigna de una vez el conjunto de permisos
/// recibido, validado contra el catálogo global de Permission (CLAUDE.md §32 "Crear rol" +
/// "Asignar permisos" combinados en un solo paso para el MVP).
/// </summary>
public class CreateRoleCommandHandler(
    IRoleRepository roles, IPermissionRepository permissions, IUnitOfWork unitOfWork, ICurrentTenantService currentTenant)
    : ICommandHandler<CreateRoleCommand, CreateRoleResult>
{
    public async Task<CreateRoleResult> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var requestedCodes = command.PermissionCodes.Select(c => c.Trim().ToLowerInvariant()).Distinct().ToList();
        var matchedPermissions = await permissions.GetByCodesAsync(requestedCodes, cancellationToken);

        if (matchedPermissions.Count != requestedCodes.Count)
        {
            var missing = requestedCodes.Except(matchedPermissions.Select(p => p.Code)).ToList();
            throw new UnknownPermissionCodesException(missing);
        }

        var role = Role.Create(tenantId, command.Name);
        roles.Add(role);

        roles.AddPermissions(matchedPermissions.Select(p => RolePermission.Create(tenantId, role.Id, p.Id)));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateRoleResult(role.Id);
    }
}
