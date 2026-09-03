namespace Fluy.Api.Models.Requests;

public record AddWorkflowStepBody(string Name, Guid ApproverRoleId);
