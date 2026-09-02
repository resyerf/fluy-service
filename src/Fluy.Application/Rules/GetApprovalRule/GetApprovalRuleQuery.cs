using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Rules.GetApprovalRule;

public record GetApprovalRuleQuery : IQuery<ApprovalRuleDetail?>, IRequiresPermission
{
    public string PermissionCode => "rules.manage";
}

public record ApprovalRuleDetail(Guid Id, decimal MinAmount, Guid SecondApproverRoleId, string SecondApproverRoleName);
