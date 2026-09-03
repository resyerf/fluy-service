using Fluy.Application.DTOs;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Identity.SetPassword;

/// <summary>
/// Redime un PasswordSetToken (creado por BootstrapTenantCommand) y deja al usuario con una
/// contraseña utilizable. Devuelve LoginResult para loguear automáticamente tras definirla —
/// mejor UX que forzar un segundo paso de login manual.
/// </summary>
public record SetPasswordCommand(string Token, string NewPassword) : ICommand<LoginResult>;
