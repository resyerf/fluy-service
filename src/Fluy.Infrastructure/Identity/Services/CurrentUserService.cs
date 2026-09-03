using Fluy.Application.Interfaces.Services;

namespace Fluy.Infrastructure.Identity.Services;

/// <summary>
/// Instancia scoped (una por request HTTP). Poblada por CurrentUserMiddleware a partir de los
/// claims del JWT, después de que UseAuthentication resuelve el usuario.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; private set; }
    public IReadOnlyCollection<string> Roles { get; private set; } = [];

    public void SetUser(Guid userId, IEnumerable<string> roles)
    {
        UserId = userId;
        Roles = roles.ToList();
    }
}
