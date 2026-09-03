using FluentValidation;

namespace Fluy.Application.Commands.Workflows.PublishWorkflowVersion;

public class PublishWorkflowVersionCommandValidator : AbstractValidator<PublishWorkflowVersionCommand>
{
    public PublishWorkflowVersionCommandValidator()
    {
        RuleFor(c => c.WorkflowVersionId).NotEmpty();
    }
}
