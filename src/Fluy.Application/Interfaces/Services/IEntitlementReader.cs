using Fluy.Application.DTOs;

namespace Fluy.Application.Interfaces.Services;

public interface IEntitlementReader
{
    Task<IReadOnlyCollection<EntitlementValue>> GetEntitlementsAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
