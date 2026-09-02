using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.ArchiveWorkflowDefinition;

/// <summary>Archiva la definición y su versión Active si tiene una — deja al tenant sin workflow activo (fallback de un solo paso, CODE.md §9.20).</summary>
public record ArchiveWorkflowDefinitionCommand(Guid WorkflowDefinitionId) : ICommand<ArchiveWorkflowDefinitionResult>, IRequiresPermission
{
    public string PermissionCode => "workflow.publish";
}

public record ArchiveWorkflowDefinitionResult(Guid WorkflowDefinitionId);
