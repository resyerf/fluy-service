using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Entities;

public class WorkflowStep : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid WorkflowVersionId { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid ApproverRoleId { get; private set; }
    public int Order { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private WorkflowStep()
    {
    }

    public static WorkflowStep Create(Guid tenantId, Guid workflowVersionId, string name, Guid approverRoleId, int order)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del paso es obligatorio.", nameof(name));
        }

        return new WorkflowStep
        {
            TenantId = tenantId,
            WorkflowVersionId = workflowVersionId,
            Name = name.Trim(),
            ApproverRoleId = approverRoleId,
            Order = order
        };
    }
}
