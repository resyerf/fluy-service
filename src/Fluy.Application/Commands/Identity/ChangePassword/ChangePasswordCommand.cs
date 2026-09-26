using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Identity.ChangePassword;

/// <summary>
/// Cambio de contraseña autenticado (usuario ya logueado, desde "Mi perfil") — distinto de
/// SetPasswordCommand, que redime un PasswordSetToken anónimo del flujo de activación.
/// </summary>
public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : ICommand<bool>;
