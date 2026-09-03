using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Workflows.SetInitialStep;

public record SetInitialStepCommand(Guid WorkflowVersionId, Guid StepId) : ICommand<SetInitialStepResult>, IRequiresPermission
{
    public string PermissionCode => "workflow.edit";
}
