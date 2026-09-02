using Fluy.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fluy.Application.Identity;

/// <summary>
/// Envío del email de activación (CODE.md §9.22) compartido por BootstrapTenantCommandHandler y
/// CreateUserCommandHandler — mismo cuerpo/link, distinto texto de bienvenida. Nunca revierte la
/// creación del usuario si el envío falla; solo informa al caller vía el bool devuelto.
/// </summary>
internal static class ActivationEmailSender
{
    public static async Task<bool> TrySendAsync(
        ITenantDirectory tenantDirectory,
        IFrontendLinkBuilder linkBuilder,
        IEmailSender emailSender,
        ILogger logger,
        Guid tenantId,
        string email,
        string rawToken,
        string welcomeMessage,
        CancellationToken cancellationToken)
    {
        try
        {
            var tenant = await tenantDirectory.FindByIdAsync(tenantId, cancellationToken)
                ?? throw new InvalidOperationException($"No se encontró el tenant '{tenantId}' en platform.Tenants.");

            var link = linkBuilder.BuildSetPasswordLink(tenant.Subdomain, rawToken);
            var body = $"""
                <p>{welcomeMessage}</p>
                <p>Activá tu cuenta definiendo tu contraseña:</p>
                <p><a href="{link}">{link}</a></p>
                <p>Este link vence en 48 horas.</p>
                """;

            await emailSender.SendAsync(email, "Activá tu cuenta en FLUY", body, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "No se pudo enviar el email de activación a {Email} del tenant {TenantId}.", email, tenantId);
            return false;
        }
    }
}
