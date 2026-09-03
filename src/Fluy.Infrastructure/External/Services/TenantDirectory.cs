using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fluy.Infrastructure.External.Services;

public class TenantDirectory(PlatformReadDbContext db, IMemoryCache cache) : ITenantDirectory
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    public async Task<TenantLookup?> FindBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"tenant-lookup:{subdomain}";

        if (cache.TryGetValue(cacheKey, out TenantLookup? cached))
        {
            return cached;
        }

        var row = await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Subdomain == subdomain, cancellationToken);
        var result = row is null ? null : new TenantLookup(row.Id, row.Subdomain, row.Status == "Active");

        cache.Set(cacheKey, result, CacheTtl);
        return result;
    }

    public async Task<TenantLookup?> FindByIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"tenant-lookup-id:{tenantId}";

        if (cache.TryGetValue(cacheKey, out TenantLookup? cached))
        {
            return cached;
        }

        var row = await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
        var result = row is null ? null : new TenantLookup(row.Id, row.Subdomain, row.Status == "Active");

        cache.Set(cacheKey, result, CacheTtl);
        return result;
    }
}
