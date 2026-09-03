using Fluy.Application.Interfaces.Services;

namespace Fluy.Infrastructure.Identity.Services;

/// <summary>
/// Instancia scoped (una por request HTTP). Poblada una única vez por TenantResolutionMiddleware.
/// </summary>
public class CurrentTenantService : ICurrentTenantService
{
    public Guid? TenantId { get; private set; }

    public void SetTenant(Guid tenantId) => TenantId = tenantId;
}
