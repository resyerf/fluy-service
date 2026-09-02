using Fluy.Domain.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fluy.Infrastructure.Persistence.Configurations;

public class WorkflowTransitionConfiguration : IEntityTypeConfiguration<WorkflowTransition>
{
    public void Configure(EntityTypeBuilder<WorkflowTransition> builder)
    {
        builder.ToTable("WorkflowTransitions");

        builder.Property(t => t.ConditionField).HasMaxLength(100);
        builder.Property(t => t.ConditionOperator).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.ConditionValue).HasColumnType("numeric(18,2)");

        builder.HasIndex(t => t.WorkflowVersionId);
        builder.HasIndex(t => t.FromStepId);
    }
}
