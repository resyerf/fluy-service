using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Commands.Identity.Login;
using Fluy.Application.Commands.Identity.SetPassword;
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
}
