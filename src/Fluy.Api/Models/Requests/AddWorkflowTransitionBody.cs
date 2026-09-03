namespace Fluy.Api.Models.Requests;

public record AddWorkflowTransitionBody(
    Guid FromStepId, Guid? ToStepId, string? ConditionField, string? ConditionOperator, decimal? ConditionValue, int Order);
