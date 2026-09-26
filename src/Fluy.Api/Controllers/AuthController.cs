using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Commands.Identity.ChangePassword;
using Fluy.Application.Commands.Identity.Login;
using Fluy.Application.Commands.Identity.SetPassword;
using Fluy.Application.Commands.Identity.UpdateProfile;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fluy.Api.Models.Requests;

namespace Fluy.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(ISender sender) : ControllerBase
{

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResult>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
            return Ok(result);
        }
        catch (AuthenticationFailedException ex)
        {
            return Unauthorized(new { detail = ex.Message });
        }
    }

    /// <summary>
    /// Redime el link enviado al usuario master de un tenant recién aprovisionado (CODE.md §9.5) —
    /// hoy ese link se arma manualmente con el token que devuelve la API interna de provisioning,
    /// ya que Notifications todavía no existe.
    /// </summary>
    [HttpPost("set-password")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResult>> SetPassword(SetPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new SetPasswordCommand(request.Token, request.NewPassword), cancellationToken);
            return Ok(result);
        }
        catch (InvalidPasswordSetTokenException ex)
        {
            return Unauthorized(new { detail = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me([FromServices] ICurrentUserService currentUser, [FromServices] ICurrentTenantService currentTenant)
    {
        return Ok(new { currentUser.UserId, currentTenant.TenantId, currentUser.Roles });
    }

    /// <summary>
    /// Actualiza el nombre del usuario autenticado desde "Mi perfil" (CLAUDE.md: sección para
    /// "actualizar información del usuario logueado en fluy-web").
    /// </summary>
    [HttpPatch("me")]
    [Authorize]
    public async Task<ActionResult<UpdateProfileResult>> UpdateProfile(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateProfileCommand(request.FullName), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cambio de contraseña autenticado desde "Mi perfil" — requiere la contraseña actual, a
    /// diferencia de SetPassword (activación anónima vía token emailado).
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new ChangePasswordCommand(request.CurrentPassword, request.NewPassword), cancellationToken);
            return NoContent();
        }
        catch (InvalidCurrentPasswordException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
    }
}
