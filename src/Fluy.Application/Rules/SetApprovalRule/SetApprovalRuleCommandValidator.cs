using FluentValidation;

namespace Fluy.Application.Rules.SetApprovalRule;

public class SetApprovalRuleCommandValidator : AbstractValidator<SetApprovalRuleCommand>
{
    public SetApprovalRuleCommandValidator()
    {
        RuleFor(c => c.MinAmount).GreaterThanOrEqualTo(0);
        RuleFor(c => c.SecondApproverRoleId).NotEmpty();
    }
}
