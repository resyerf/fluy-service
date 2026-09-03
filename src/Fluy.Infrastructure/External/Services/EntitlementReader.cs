using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fluy.Infrastructure.External.Services;

public class EntitlementReader(PlatformReadDbContext db, IMemoryCache cache) : IEntitlementReader
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);
    private static readonly string[] InactiveStatuses = ["Cancelled", "Expired"];

    public async Task<IReadOnlyCollection<EntitlementValue>> GetEntitlementsAsync(
        Guid tenantId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"entitlements:{tenantId}";

        if (cache.TryGetValue(cacheKey, out IReadOnlyCollection<EntitlementValue>? cached))
        {
            return cached!;
        }

        var subscription = await db.Subscriptions.AsNoTracking()
            .FirstOrDefaultAsync(s => s.TenantId == tenantId, cancellationToken);

        List<EntitlementValue> result;

        if (subscription is null || InactiveStatuses.Contains(subscription.Status))
        {
            result = [];
        }
        else
        {
            result = await (
                    from planFeature in db.PlanFeatures.AsNoTracking()
                    join feature in db.Features.AsNoTracking() on planFeature.FeatureId equals feature.Id
                    where planFeature.PlanId == subscription.PlanId
                    select new EntitlementValue(feature.Code, planFeature.Value))
                .ToListAsync(cancellationToken);
        }

        cache.Set(cacheKey, (IReadOnlyCollection<EntitlementValue>)result, CacheTtl);
        return result;
    }
}
