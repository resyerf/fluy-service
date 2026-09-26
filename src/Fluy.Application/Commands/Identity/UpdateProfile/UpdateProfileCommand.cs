using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Identity.UpdateProfile;

/// <summary>
/// Actualiza el nombre del usuario autenticado, desde "Mi perfil". El email no es editable acá —
/// es el identificador de login, cambiarlo implicaría validar unicidad y re-verificación, fuera
/// de alcance de este comando.
/// </summary>
public record UpdateProfileCommand(string FullName) : ICommand<UpdateProfileResult>;
