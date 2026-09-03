using Fluy.Domain.Common;
using Fluy.SharedKernel;
using Fluy.Domain.Enums;

namespace Fluy.Domain.Entities;

/// <summary>
/// Una ejecución concreta de un <see cref="WorkflowVersion"/> para una Request (CLAUDE.md §14,
/// "Workflow History" de CODE.md §4.19). <see cref="CurrentStepId"/> nulo significa completada.
/// </summary>
public class WorkflowInstance : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid WorkflowDefinitionId { get; private set; }
    public Guid WorkflowVersionId { get; private set; }
    public Guid RequestId { get; private set; }
    public Guid? CurrentStepId { get; private set; }
    public WorkflowInstanceStatus Status { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private WorkflowInstance()
    {
    }

    public static WorkflowInstance Start(
        Guid tenantId, Guid workflowDefinitionId, Guid workflowVersionId, Guid requestId, Guid initialStepId) => new()
        {
            TenantId = tenantId,
            WorkflowDefinitionId = workflowDefinitionId,
            WorkflowVersionId = workflowVersionId,
            RequestId = requestId,
            CurrentStepId = initialStepId,
            Status = WorkflowInstanceStatus.Running
        };

    public void MoveTo(Guid nextStepId)
    {
        EnsureRunning();
        CurrentStepId = nextStepId;
    }

    public void Complete(DateTimeOffset now)
    {
        EnsureRunning();
        CurrentStepId = null;
        Status = WorkflowInstanceStatus.Completed;
        CompletedAt = now;
    }

    public void Cancel()
    {
        if (Status == WorkflowInstanceStatus.Running)
        {
            Status = WorkflowInstanceStatus.Cancelled;
        }
    }

    private void EnsureRunning()
    {
        if (Status != WorkflowInstanceStatus.Running)
        {
            throw new InvalidOperationException($"Esta instancia de workflow ya no está en curso (estado actual: {Status}).");
        }
    }
}
