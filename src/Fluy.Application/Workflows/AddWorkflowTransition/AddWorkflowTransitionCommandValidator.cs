using Fluy.Domain.Workflows;
using FluentValidation;

namespace Fluy.Application.Workflows.AddWorkflowTransition;

public class AddWorkflowTransitionCommandValidator : AbstractValidator<AddWorkflowTransitionCommand>
{
    public AddWorkflowTransitionCommandValidator()
    {
        RuleFor(c => c.WorkflowVersionId).NotEmpty();
        RuleFor(c => c.FromStepId).NotEmpty();

        RuleFor(c => c.ConditionOperator)
            .Must(op => op is null || Enum.TryParse<WorkflowConditionOperator>(op, out _))
            .WithMessage("ConditionOperator debe ser uno de: GreaterThanOrEqual, GreaterThan, LessThanOrEqual, LessThan, Equal.");
    }
}
