using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Rules.GetApprovalRule;

public class GetApprovalRuleQueryHandler(IApprovalRuleRepository approvalRules) : IQueryHandler<GetApprovalRuleQuery, ApprovalRuleDetail?>
{
    public Task<ApprovalRuleDetail?> Handle(GetApprovalRuleQuery query, CancellationToken cancellationToken) =>
        approvalRules.GetDetailAsync(cancellationToken);
}
