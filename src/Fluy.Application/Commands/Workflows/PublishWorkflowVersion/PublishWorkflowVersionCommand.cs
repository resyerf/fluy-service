using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Workflows.PublishWorkflowVersion;

public record PublishWorkflowVersionCommand(Guid WorkflowVersionId) : ICommand<PublishWorkflowVersionResult>, IRequiresPermission
{
    public string PermissionCode => "workflow.publish";
}
