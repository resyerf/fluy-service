using System.Security.Claims;
using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Identity;
using Fluy.SharedKernel.Dispatching;
using Fluy.SharedKernel.Security;

namespace Fluy.Application.Identity.Login;

public class LoginCommandHandler(
    IUserRepository users,
    ICurrentTenantService currentTenant,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : ICommandHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId ?? throw new AuthenticationFailedException();
        var email = command.Email.Trim().ToLowerInvariant();

        var user = await users.GetByEmailAsync(email, cancellationToken);

        if (user is null || user.Status != UserStatus.Active || !passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            throw new AuthenticationFailedException();
        }

        var roleNames = await users.GetRoleNamesAsync(user.Id, cancellationToken);

        List<Claim> claims =
        [
            new("sub", user.Id.ToString()),
            new("email", user.Email),
            new("jti", Guid.NewGuid().ToString()),
            new("tenant_id", tenantId.ToString())
        ];
        claims.AddRange(roleNames.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = jwtTokenGenerator.GenerateToken(claims);

        return new LoginResult(token, user.Id, user.Email, user.FullName, roleNames);
    }
}
