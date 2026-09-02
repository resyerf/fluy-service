using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.SetInitialStep;

public record SetInitialStepCommand(Guid WorkflowVersionId, Guid StepId) : ICommand<SetInitialStepResult>, IRequiresPermission
{
    public string PermissionCode => "workflow.edit";
}

public record SetInitialStepResult(Guid WorkflowVersionId, Guid StepId);
