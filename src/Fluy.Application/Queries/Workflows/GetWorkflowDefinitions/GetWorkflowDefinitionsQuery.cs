using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Workflows.GetWorkflowDefinitions;

public record GetWorkflowDefinitionsQuery : IQuery<IReadOnlyCollection<WorkflowDefinitionSummary>>, IRequiresPermission
{
    public string PermissionCode => "workflow.edit";
}
