using FluentValidation;

namespace Fluy.Application.Workflows.ArchiveWorkflowDefinition;

public class ArchiveWorkflowDefinitionCommandValidator : AbstractValidator<ArchiveWorkflowDefinitionCommand>
{
    public ArchiveWorkflowDefinitionCommandValidator()
    {
        RuleFor(c => c.WorkflowDefinitionId).NotEmpty();
    }
}
