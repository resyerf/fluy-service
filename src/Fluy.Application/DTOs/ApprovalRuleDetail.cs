namespace Fluy.Application.DTOs;

public record ApprovalRuleDetail(Guid Id, decimal MinAmount, Guid SecondApproverRoleId, string SecondApproverRoleName);
