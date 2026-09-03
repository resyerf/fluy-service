using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Identity.AssignRole;

public record AssignRoleCommand(Guid UserId, Guid RoleId, Guid? BranchId, Guid? DepartmentId)
    : ICommand<AssignRoleResult>, IRequiresPermission
{
    public string PermissionCode => "users.manage";
}
