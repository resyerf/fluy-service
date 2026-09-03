using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Workflows.GetWorkflowVersionDetail;

public record GetWorkflowVersionDetailQuery(Guid WorkflowVersionId)
    : IQuery<WorkflowVersionDetail>, IRequiresPermission
{
    public string PermissionCode => "workflow.edit";
}
