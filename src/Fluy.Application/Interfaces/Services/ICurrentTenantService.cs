namespace Fluy.Application.Interfaces.Services;

/// <summary>
/// Único punto de verdad sobre el tenant de la request actual. Poblado exclusivamente por
/// TenantResolutionMiddleware (Fluy.Api) a partir del subdominio — ningún handler debe recibir
/// TenantId como parámetro de entrada del cliente (CLAUDE.md §37, CODE.md §4.24).
/// </summary>
public interface ICurrentTenantService
{
    Guid? TenantId { get; }

    void SetTenant(Guid tenantId);
}
