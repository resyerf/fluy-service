using Fluy.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fluy.Infrastructure.Persistence.Configurations;

public class PasswordSetTokenConfiguration : IEntityTypeConfiguration<PasswordSetToken>
{
    public void Configure(EntityTypeBuilder<PasswordSetToken> builder)
    {
        builder.ToTable("PasswordSetTokens");

        builder.Property(t => t.TokenHash).IsRequired().HasMaxLength(64);

        builder.HasIndex(t => t.TokenHash).IsUnique();
        builder.HasIndex(t => new { t.TenantId, t.UserId });
    }
}
