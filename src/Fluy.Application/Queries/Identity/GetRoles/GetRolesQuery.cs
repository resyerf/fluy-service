using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Identity.GetRoles;

public record GetRolesQuery : IQuery<IReadOnlyCollection<RoleDetail>>, IRequiresPermission
{
    public string PermissionCode => "roles.manage";
}
