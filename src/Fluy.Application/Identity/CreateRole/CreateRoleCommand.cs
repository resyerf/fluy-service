using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.CreateRole;

public record CreateRoleCommand(string Name, IReadOnlyCollection<string> PermissionCodes)
    : ICommand<CreateRoleResult>, IRequiresPermission
{
    public string PermissionCode => "roles.manage";
}

public record CreateRoleResult(Guid RoleId);
