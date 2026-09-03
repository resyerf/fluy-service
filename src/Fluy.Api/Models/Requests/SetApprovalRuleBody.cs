namespace Fluy.Api.Models.Requests;

public record SetApprovalRuleBody(decimal MinAmount, Guid SecondApproverRoleId);
