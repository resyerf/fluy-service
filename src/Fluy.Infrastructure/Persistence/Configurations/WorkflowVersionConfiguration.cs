using Fluy.Domain.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fluy.Infrastructure.Persistence.Configurations;

public class WorkflowVersionConfiguration : IEntityTypeConfiguration<WorkflowVersion>
{
    public void Configure(EntityTypeBuilder<WorkflowVersion> builder)
    {
        builder.ToTable("WorkflowVersions");

        builder.Property(v => v.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(v => v.WorkflowDefinitionId);
        builder.HasIndex(v => new { v.TenantId, v.Status });
    }
}
