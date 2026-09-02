using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Identity;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.AssignRole;

public class AssignRoleCommandHandler(
    IUserRoleRepository userRoles, IUserRepository users, IRoleRepository roles, IUnitOfWork unitOfWork,
    ICurrentTenantService currentTenant) : ICommandHandler<AssignRoleCommand, AssignRoleResult>
{
    public async Task<AssignRoleResult> Handle(AssignRoleCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var userExists = await users.ExistsAsync(command.UserId, cancellationToken);
        if (!userExists)
        {
            throw new UserNotFoundException(command.UserId);
        }

        var roleExists = await roles.ExistsAsync(command.RoleId, cancellationToken);
        if (!roleExists)
        {
            throw new RoleNotFoundException(command.RoleId);
        }

        var userRole = UserRole.Create(tenantId, command.UserId, command.RoleId, command.BranchId, command.DepartmentId);
        userRoles.Add(userRole);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AssignRoleResult(userRole.Id);
    }
}
