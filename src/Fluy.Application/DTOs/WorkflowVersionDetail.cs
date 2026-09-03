namespace Fluy.Application.DTOs;

public record WorkflowVersionDetail(
    Guid Id, Guid WorkflowDefinitionId, int VersionNumber, string Status, Guid? InitialStepId,
    IReadOnlyCollection<WorkflowStepDetail> Steps, IReadOnlyCollection<WorkflowTransitionDetail> Transitions);
