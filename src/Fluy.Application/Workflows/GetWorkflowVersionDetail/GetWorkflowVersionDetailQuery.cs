using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.GetWorkflowVersionDetail;

public record GetWorkflowVersionDetailQuery(Guid WorkflowVersionId)
    : IQuery<WorkflowVersionDetail>, IRequiresPermission
{
    public string PermissionCode => "workflow.edit";
}

public record WorkflowStepDetail(Guid Id, string Name, Guid ApproverRoleId, string ApproverRoleName, int Order);

public record WorkflowTransitionDetail(
    Guid Id, Guid FromStepId, Guid? ToStepId,
    string? ConditionField, string? ConditionOperator, decimal? ConditionValue, int Order);

public record WorkflowVersionDetail(
    Guid Id, Guid WorkflowDefinitionId, int VersionNumber, string Status, Guid? InitialStepId,
    IReadOnlyCollection<WorkflowStepDetail> Steps, IReadOnlyCollection<WorkflowTransitionDetail> Transitions);
