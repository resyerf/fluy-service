using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fluy.Infrastructure.Persistence.Configurations;

public class RequestFieldConfiguration : IEntityTypeConfiguration<RequestField>
{
    public void Configure(EntityTypeBuilder<RequestField> builder)
    {
        builder.ToTable("RequestFields");

        builder.Property(f => f.Key).IsRequired().HasMaxLength(100);
        builder.Property(f => f.Value).HasMaxLength(4000);

        builder.HasIndex(f => f.RequestId);
    }
}
