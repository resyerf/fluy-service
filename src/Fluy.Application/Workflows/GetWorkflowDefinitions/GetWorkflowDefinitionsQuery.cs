using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.GetWorkflowDefinitions;

public record GetWorkflowDefinitionsQuery : IQuery<IReadOnlyCollection<WorkflowDefinitionSummary>>, IRequiresPermission
{
    public string PermissionCode => "workflow.edit";
}

public record WorkflowVersionSummary(Guid Id, int VersionNumber, string Status);

public record WorkflowDefinitionSummary(
    Guid Id, string Name, string Description, string Status, IReadOnlyCollection<WorkflowVersionSummary> Versions);
