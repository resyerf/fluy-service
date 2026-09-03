using FluentValidation;

namespace Fluy.Application.Commands.Workflows.AddWorkflowStep;

public class AddWorkflowStepCommandValidator : AbstractValidator<AddWorkflowStepCommand>
{
    public AddWorkflowStepCommandValidator()
    {
        RuleFor(c => c.WorkflowVersionId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.ApproverRoleId).NotEmpty();
    }
}
