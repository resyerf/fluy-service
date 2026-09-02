using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.CreateUser;

public record CreateUserCommand(string Email, string FullName) : ICommand<CreateUserResult>, IRequiresPermission
{
    public string PermissionCode => "users.manage";
}

/// <summary>El token de activación se envía por email (CODE.md §9.22) — ya no viaja en la respuesta (igual que BootstrapTenantResult).</summary>
public record CreateUserResult(Guid UserId, bool ActivationEmailSent);
