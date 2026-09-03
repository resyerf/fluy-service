using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fluy.Application.Interfaces.Services;

namespace Fluy.Api.Middlewares;

/// <summary>
/// Traduce los claims del JWT ya validado por UseAuthentication a ICurrentUserService.
/// Debe registrarse después de UseAuthentication y antes de UseAuthorization.
/// </summary>
public class CurrentUserMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUser)
    {
        currentUser.SetRequestContext(context.TraceIdentifier, context.Connection.RemoteIpAddress?.ToString());

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub) ?? context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is not null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
                currentUser.SetUser(userId, roles);
            }
        }

        await next(context);
    }
}
