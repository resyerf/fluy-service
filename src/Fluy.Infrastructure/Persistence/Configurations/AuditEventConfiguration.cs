using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fluy.Infrastructure.Persistence.Configurations;

public class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        builder.ToTable("AuditEvents");

        builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
        builder.Property(a => a.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.PreviousState).HasMaxLength(50);
        builder.Property(a => a.NewState).HasMaxLength(50);
        builder.Property(a => a.Metadata).HasMaxLength(4000);
        builder.Property(a => a.IpAddress).HasMaxLength(45);
        builder.Property(a => a.CorrelationId).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Reason).HasMaxLength(1000);
        builder.Property(a => a.Comment).HasMaxLength(2000);

        builder.HasIndex(a => new { a.EntityType, a.EntityId });
        builder.HasIndex(a => a.TenantId);
    }
}
