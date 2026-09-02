using Fluy.Application.Common.Interfaces;
using Fluy.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Fluy.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Defensa en profundidad (CODE.md §4.24): ninguna entidad tenant-scoped puede insertarse o
/// modificarse con un TenantId distinto al de la request actual, aunque el código de aplicación
/// tenga un bug. El Global Query Filter (ver ApplicationDbContext) cubre lecturas; esto cubre escrituras.
/// </summary>
public class TenantIntegrityInterceptor(ICurrentTenantService currentTenant) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        EnsureTenantIntegrity(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        EnsureTenantIntegrity(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void EnsureTenantIntegrity(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
            {
                continue;
            }

            if (currentTenant.TenantId is null || entry.Entity.TenantId != currentTenant.TenantId)
            {
                throw new InvalidOperationException(
                    $"Intento de escritura cross-tenant detectado en {entry.Entity.GetType().Name}.");
            }
        }
    }
}
