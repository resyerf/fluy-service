using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Workflows.AddWorkflowStep;

/// <summary>El primer paso agregado a una versión se vuelve automáticamente su paso inicial (CODE.md §9.20).</summary>
public record AddWorkflowStepCommand(Guid WorkflowVersionId, string Name, Guid ApproverRoleId)
    : ICommand<AddWorkflowStepResult>, IRequiresPermission
{
    public string PermissionCode => "workflow.edit";
}
