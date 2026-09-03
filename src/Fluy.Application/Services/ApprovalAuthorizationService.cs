using Fluy.Application.Common.Exceptions;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;

namespace Fluy.Application.Services;

/// <summary>
/// `IRequiresPermission`/`PermissionChecker` (D19) ya validan que el usuario tiene `request.approve`
/// en general; esto valida el requisito adicional de un tier 2 gateado por `ApprovalRule` (CODE.md
/// §9.19): solo alguien con <see cref="Approval.RequiredRoleId"/> puede decidir ese paso concreto.
/// Compartido por Approve/Reject/RequestCorrection para no triplicar la misma consulta.
/// </summary>
internal sealed class ApprovalAuthorizationService(IUserRoleRepository userRoles, IRoleRepository roles) : IApprovalAuthorizationService
{
    public async Task EnsureCanDecideAsync(Approval approval, Guid userId, CancellationToken cancellationToken)
    {
        if (approval.RequiredRoleId is null)
        {
            return;
        }

        var hasRole = await userRoles.HasRoleAsync(userId, approval.RequiredRoleId.Value, cancellationToken);

        if (!hasRole)
        {
            var roleName = await roles.GetNameAsync(approval.RequiredRoleId.Value, cancellationToken)
                ?? approval.RequiredRoleId.Value.ToString();

            throw new RequiredRoleNotHeldException(roleName);
        }
    }
}
