namespace Fluy.Application.DTOs;

public record WorkflowStepDetail(Guid Id, string Name, Guid ApproverRoleId, string ApproverRoleName, int Order);
