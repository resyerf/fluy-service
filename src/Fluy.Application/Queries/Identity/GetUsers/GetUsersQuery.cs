using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Identity.GetUsers;

public record GetUsersQuery : IQuery<IReadOnlyCollection<UserDetail>>, IRequiresPermission
{
    public string PermissionCode => "users.manage";
}
