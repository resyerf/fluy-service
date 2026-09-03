namespace Fluy.Application.DTOs;

public record WorkflowDefinitionSummary(
    Guid Id, string Name, string Description, string Status, IReadOnlyCollection<WorkflowVersionSummary> Versions);
