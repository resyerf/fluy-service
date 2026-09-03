namespace Fluy.Application.DTOs;

/// <summary>El token de activación se envía por email (CODE.md §9.22) — ya no viaja en la respuesta (igual que BootstrapTenantResult).</summary>
public record CreateUserResult(Guid UserId, bool ActivationEmailSent);
