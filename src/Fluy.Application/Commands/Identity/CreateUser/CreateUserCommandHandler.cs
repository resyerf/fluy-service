using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Application.Services;
using Fluy.Domain.Entities;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;
using Fluy.SharedKernel.Security;
using Microsoft.Extensions.Logging;

namespace Fluy.Application.Commands.Identity.CreateUser;

/// <summary>
/// Crea un usuario del tenant sin contraseña utilizable, igual patrón que BootstrapTenantCommandHandler
/// pero para altas posteriores a la del usuario master (CLAUDE.md §32 "Crear usuario"). El link de
/// activación se envía por email (CODE.md §9.22) vía el mismo helper compartido.
/// </summary>
public class CreateUserCommandHandler(
    IUserRepository users,
    IUnitOfWork unitOfWork,
    ICurrentTenantService currentTenant,
    ITenantDirectory tenantDirectory,
    IFrontendLinkBuilder linkBuilder,
    IEmailSender emailSender,
    IPasswordHasher passwordHasher,
    IDateTime dateTime,
    ILogger<CreateUserCommandHandler> logger) : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(48);

    public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;
        var email = command.Email.Trim().ToLowerInvariant();

        var alreadyRegistered = await users.ExistsByEmailAsync(email, cancellationToken);
        if (alreadyRegistered)
        {
            throw new EmailAlreadyRegisteredException(email);
        }

        var unusablePasswordHash = passwordHasher.Hash(Guid.NewGuid().ToString());
        var user = User.Create(tenantId, email, command.FullName, unusablePasswordHash);
        users.Add(user);

        var rawToken = TokenHasher.GenerateRawToken();
        var token = PasswordSetToken.Create(tenantId, user.Id, TokenHasher.Hash(rawToken), dateTime.UtcNow.Add(TokenLifetime));
        users.AddPasswordSetToken(token);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var emailSent = await ActivationEmailSender.TrySendAsync(
            tenantDirectory, linkBuilder, emailSender, logger, tenantId, email, rawToken,
            "Te crearon una cuenta en FLUY.", cancellationToken);

        return new CreateUserResult(user.Id, emailSent);
    }
}
