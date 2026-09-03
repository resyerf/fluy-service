using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fluy.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.Property(d => d.FileName).IsRequired().HasMaxLength(255);
        builder.Property(d => d.ContentType).HasMaxLength(200);
        builder.Property(d => d.StorageKey).IsRequired().HasMaxLength(500);

        builder.HasIndex(d => d.RequestId);
        builder.HasIndex(d => d.TenantId);
    }
}
