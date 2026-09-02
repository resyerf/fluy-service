using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.GetUsers;

public record GetUsersQuery : IQuery<IReadOnlyCollection<UserDetail>>, IRequiresPermission
{
    public string PermissionCode => "users.manage";
}

public record UserDetail(
    Guid Id,
    string Email,
    string FullName,
    string Status,
    IReadOnlyCollection<UserRoleDetail> Roles);

public record UserRoleDetail(Guid UserRoleId, Guid RoleId, string RoleName, Guid? BranchId, Guid? DepartmentId);
