namespace Fluy.Application.DTOs;

/// <summary>
/// El token de activación ya no viaja en la respuesta (CODE.md §9.22, Notifications real): se envía
/// por email al usuario master. <see cref="ActivationEmailSent"/> es honesto sobre si el envío
/// falló (ej. SMTP caído) — la creación del usuario/tenant nunca se revierte por eso, pero el
/// caller debe poder saber que hace falta reenviar el link manualmente.
/// </summary>
public record BootstrapTenantResult(Guid MasterUserId, bool ActivationEmailSent);
