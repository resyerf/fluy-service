using Fluy.Application.DTOs;

namespace Fluy.Application.Interfaces.Services;

public interface ITenantDirectory
{
    Task<TenantLookup?> FindBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default);
    Task<TenantLookup?> FindByIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
