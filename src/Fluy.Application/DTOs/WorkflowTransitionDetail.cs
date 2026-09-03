namespace Fluy.Application.DTOs;

public record WorkflowTransitionDetail(
    Guid Id, Guid FromStepId, Guid? ToStepId,
    string? ConditionField, string? ConditionOperator, decimal? ConditionValue, int Order);
