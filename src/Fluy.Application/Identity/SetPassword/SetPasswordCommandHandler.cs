using System.Security.Claims;
using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Application.Identity.Login;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;
using Fluy.SharedKernel.Security;

namespace Fluy.Application.Identity.SetPassword;

public class SetPasswordCommandHandler(
    IUserRepository users,
    IUnitOfWork unitOfWork,
    ICurrentTenantService currentTenant,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IDateTime dateTime) : ICommandHandler<SetPasswordCommand, LoginResult>
{
    public async Task<LoginResult> Handle(SetPasswordCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId ?? throw new InvalidPasswordSetTokenException();
        var tokenHash = TokenHasher.Hash(command.Token);

        var token = await users.GetPasswordSetTokenByHashAsync(tokenHash, cancellationToken);
        if (token is null || !token.IsValid(dateTime.UtcNow))
        {
            throw new InvalidPasswordSetTokenException();
        }

        var user = await users.GetByIdAsync(token.UserId, cancellationToken)
            ?? throw new InvalidPasswordSetTokenException();

        user.ChangePassword(passwordHasher.Hash(command.NewPassword));
        token.MarkUsed(dateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var roleNames = await users.GetRoleNamesAsync(user.Id, cancellationToken);

        List<Claim> claims =
        [
            new("sub", user.Id.ToString()),
            new("email", user.Email),
            new("jti", Guid.NewGuid().ToString()),
            new("tenant_id", tenantId.ToString())
        ];
        claims.AddRange(roleNames.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwt = jwtTokenGenerator.GenerateToken(claims);

        return new LoginResult(jwt, user.Id, user.Email, user.FullName, roleNames);
    }
}
