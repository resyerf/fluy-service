namespace Fluy.Application.DTOs;

/// <summary>
/// Lectura cacheada de platform.Tenants (CODE.md §9.4, excepción documentada #1). fluy-service
/// nunca escribe Tenants — solo TenantResolutionMiddleware lo consulta, en cada request, para
/// resolver el subdominio al TenantId/estado.
/// </summary>
public record TenantLookup(Guid Id, string Subdomain, bool IsActive);
