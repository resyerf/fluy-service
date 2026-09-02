using Microsoft.EntityFrameworkCore;

namespace Fluy.Infrastructure.PlatformAccess;

/// <summary>
/// DbContext de solo lectura contra el schema "platform" (propiedad de fluy-admin-service, misma
/// instancia de Postgres — CODE.md §9.4). Nunca genera migraciones: el schema "platform" lo
/// gestiona exclusivamente FluyAdmin.Infrastructure. Solo mapea las columnas que fluy-service
/// necesita leer, no las tablas completas.
/// </summary>
public class PlatformReadDbContext(DbContextOptions<PlatformReadDbContext> options) : DbContext(options)
{
    public DbSet<TenantRow> Tenants => Set<TenantRow>();
    public DbSet<SubscriptionRow> Subscriptions => Set<SubscriptionRow>();
    public DbSet<PlanFeatureRow> PlanFeatures => Set<PlanFeatureRow>();
    public DbSet<FeatureRow> Features => Set<FeatureRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("platform");

        modelBuilder.Entity<TenantRow>(builder =>
        {
            builder.ToTable("Tenants");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Subdomain);
            builder.Property(t => t.Status);
        });

        modelBuilder.Entity<SubscriptionRow>(builder =>
        {
            builder.ToTable("Subscriptions");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.TenantId);
            builder.Property(s => s.PlanId);
            builder.Property(s => s.Status);
        });

        modelBuilder.Entity<PlanFeatureRow>(builder =>
        {
            builder.ToTable("PlanFeatures");
            builder.HasKey(pf => pf.Id);
            builder.Property(pf => pf.PlanId);
            builder.Property(pf => pf.FeatureId);
            builder.Property(pf => pf.Value);
        });

        modelBuilder.Entity<FeatureRow>(builder =>
        {
            builder.ToTable("Features");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Code);
        });

        base.OnModelCreating(modelBuilder);
    }
}
