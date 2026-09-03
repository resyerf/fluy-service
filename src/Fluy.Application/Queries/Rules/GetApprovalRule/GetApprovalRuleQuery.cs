using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Rules.GetApprovalRule;

public record GetApprovalRuleQuery : IQuery<ApprovalRuleDetail?>, IRequiresPermission
{
    public string PermissionCode => "rules.manage";
}
