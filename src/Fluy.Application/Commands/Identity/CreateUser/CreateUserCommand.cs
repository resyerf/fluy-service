using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Identity.CreateUser;

public record CreateUserCommand(string Email, string FullName) : ICommand<CreateUserResult>, IRequiresPermission
{
    public string PermissionCode => "users.manage";
}
