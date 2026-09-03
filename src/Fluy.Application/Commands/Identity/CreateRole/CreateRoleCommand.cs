using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Identity.CreateRole;

public record CreateRoleCommand(string Name, IReadOnlyCollection<string> PermissionCodes)
    : ICommand<CreateRoleResult>, IRequiresPermission
{
    public string PermissionCode => "roles.manage";
}
