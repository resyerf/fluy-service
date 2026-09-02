using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.PublishWorkflowVersion;

public record PublishWorkflowVersionCommand(Guid WorkflowVersionId) : ICommand<PublishWorkflowVersionResult>, IRequiresPermission
{
    public string PermissionCode => "workflow.publish";
}

public record PublishWorkflowVersionResult(Guid WorkflowDefinitionId, Guid WorkflowVersionId);
