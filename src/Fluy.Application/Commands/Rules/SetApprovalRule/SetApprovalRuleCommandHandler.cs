using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Rules.SetApprovalRule;

public class SetApprovalRuleCommandHandler(
    IApprovalRuleRepository approvalRules, IRoleRepository roles, IUnitOfWork unitOfWork, ICurrentTenantService currentTenant)
    : ICommandHandler<SetApprovalRuleCommand, SetApprovalRuleResult>
{
    public async Task<SetApprovalRuleResult> Handle(SetApprovalRuleCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var roleExists = await roles.ExistsAsync(command.SecondApproverRoleId, cancellationToken);
        if (!roleExists)
        {
            throw new RoleNotFoundException(command.SecondApproverRoleId);
        }

        var rule = await approvalRules.GetForTenantAsync(tenantId, cancellationToken);

        if (rule is null)
        {
            rule = ApprovalRule.Create(tenantId, command.MinAmount, command.SecondApproverRoleId);
            approvalRules.Add(rule);
        }
        else
        {
            rule.Update(command.MinAmount, command.SecondApproverRoleId);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SetApprovalRuleResult(rule.Id);
    }
}
