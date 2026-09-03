using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Rules.GetApprovalRule;

public class GetApprovalRuleQueryHandler(IApprovalRuleRepository approvalRules) : IQueryHandler<GetApprovalRuleQuery, ApprovalRuleDetail?>
{
    public Task<ApprovalRuleDetail?> Handle(GetApprovalRuleQuery query, CancellationToken cancellationToken) =>
        approvalRules.GetDetailAsync(cancellationToken);
}
