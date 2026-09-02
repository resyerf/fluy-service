using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.AssignRole;

public record AssignRoleCommand(Guid UserId, Guid RoleId, Guid? BranchId, Guid? DepartmentId)
    : ICommand<AssignRoleResult>, IRequiresPermission
{
    public string PermissionCode => "users.manage";
}

public record AssignRoleResult(Guid UserRoleId);
