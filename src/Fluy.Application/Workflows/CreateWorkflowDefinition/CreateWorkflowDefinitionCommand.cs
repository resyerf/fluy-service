using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.CreateWorkflowDefinition;

/// <summary>Crea la definición y su primera versión en Draft (CODE.md §9.20) — no hay un paso separado de "crear versión".</summary>
public record CreateWorkflowDefinitionCommand(string Name, string Description)
    : ICommand<CreateWorkflowDefinitionResult>, IRequiresPermission
{
    public string PermissionCode => "workflow.create";
}

public record CreateWorkflowDefinitionResult(Guid WorkflowDefinitionId, Guid DraftVersionId);
