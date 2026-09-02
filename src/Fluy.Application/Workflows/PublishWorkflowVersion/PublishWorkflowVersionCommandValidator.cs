using FluentValidation;

namespace Fluy.Application.Workflows.PublishWorkflowVersion;

public class PublishWorkflowVersionCommandValidator : AbstractValidator<PublishWorkflowVersionCommand>
{
    public PublishWorkflowVersionCommandValidator()
    {
        RuleFor(c => c.WorkflowVersionId).NotEmpty();
    }
}
