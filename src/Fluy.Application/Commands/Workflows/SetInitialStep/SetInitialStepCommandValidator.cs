using FluentValidation;

namespace Fluy.Application.Commands.Workflows.SetInitialStep;

public class SetInitialStepCommandValidator : AbstractValidator<SetInitialStepCommand>
{
    public SetInitialStepCommandValidator()
    {
        RuleFor(c => c.WorkflowVersionId).NotEmpty();
        RuleFor(c => c.StepId).NotEmpty();
    }
}
