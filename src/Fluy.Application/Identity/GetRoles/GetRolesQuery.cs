using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.GetRoles;

public record GetRolesQuery : IQuery<IReadOnlyCollection<RoleDetail>>, IRequiresPermission
{
    public string PermissionCode => "roles.manage";
}

public record RoleDetail(Guid Id, string Name, bool IsSystemRole, IReadOnlyCollection<string> PermissionCodes);
