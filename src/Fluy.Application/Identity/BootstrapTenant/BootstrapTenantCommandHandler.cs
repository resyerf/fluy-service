using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Identity;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;
using Fluy.SharedKernel.Security;
using Microsoft.Extensions.Logging;

namespace Fluy.Application.Identity.BootstrapTenant;

/// <summary>
/// Crea el rol TenantAdmin (con el catálogo de permisos vigente — D17 de CODE.md §10) y el usuario
/// master de un tenant recién aprovisionado, junto con un PasswordSetToken de un solo uso. El
/// usuario master se crea sin contraseña utilizable: solo puede activarse redimiendo ese token
/// vía SetPasswordCommand. El link de activación se envía por email (CODE.md §9.22) — si el envío
/// falla, se registra el error pero no se revierte la creación del usuario/tenant (el token ya
/// quedó persistido y puede reenviarse manualmente).
/// </summary>
public class BootstrapTenantCommandHandler(
    IUserRepository users,
    IRoleRepository roles,
    IPermissionRepository permissionsRepo,
    IUserRoleRepository userRoles,
    IUnitOfWork unitOfWork,
    ICurrentTenantService currentTenant,
    ITenantDirectory tenantDirectory,
    IFrontendLinkBuilder linkBuilder,
    IEmailSender emailSender,
    IPasswordHasher passwordHasher,
    IDateTime dateTime,
    ILogger<BootstrapTenantCommandHandler> logger) : ICommandHandler<BootstrapTenantCommand, BootstrapTenantResult>
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(48);

    public async Task<BootstrapTenantResult> Handle(BootstrapTenantCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId ?? throw new InvalidOperationException(
            "BootstrapTenantCommand requiere que ICurrentTenantService.TenantId esté fijado antes de despachar.");

        var email = command.MasterEmail.Trim().ToLowerInvariant();

        var alreadyProvisioned = await users.ExistsByEmailAsync(email, cancellationToken);
        if (alreadyProvisioned)
        {
            throw new EmailAlreadyRegisteredException(email);
        }

        var adminRole = Role.Create(tenantId, "TenantAdmin", isSystemRole: true);
        roles.Add(adminRole);

        var permissions = await permissionsRepo.GetAllAsync(cancellationToken);
        roles.AddPermissions(permissions.Select(p => RolePermission.Create(tenantId, adminRole.Id, p.Id)));

        var unusablePasswordHash = passwordHasher.Hash(Guid.NewGuid().ToString());
        var masterUser = User.Create(tenantId, email, command.MasterFullName, unusablePasswordHash);
        users.Add(masterUser);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        userRoles.Add(UserRole.Create(tenantId, masterUser.Id, adminRole.Id));

        var rawToken = TokenHasher.GenerateRawToken();
        var token = PasswordSetToken.Create(tenantId, masterUser.Id, TokenHasher.Hash(rawToken), dateTime.UtcNow.Add(TokenLifetime));
        users.AddPasswordSetToken(token);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var emailSent = await ActivationEmailSender.TrySendAsync(
            tenantDirectory, linkBuilder, emailSender, logger, tenantId, email, rawToken,
            "Bienvenido a FLUY. Sos el administrador de tu empresa.", cancellationToken);

        return new BootstrapTenantResult(masterUser.Id, emailSent);
    }
}
