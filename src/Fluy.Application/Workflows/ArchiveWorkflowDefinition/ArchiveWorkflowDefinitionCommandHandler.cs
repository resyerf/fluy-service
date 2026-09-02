using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.ArchiveWorkflowDefinition;

public class ArchiveWorkflowDefinitionCommandHandler(
    IWorkflowDefinitionRepository definitions, IWorkflowVersionRepository versions, IUnitOfWork unitOfWork)
    : ICommandHandler<ArchiveWorkflowDefinitionCommand, ArchiveWorkflowDefinitionResult>
{
    public async Task<ArchiveWorkflowDefinitionResult> Handle(ArchiveWorkflowDefinitionCommand command, CancellationToken cancellationToken)
    {
        var definition = await definitions.GetByIdAsync(command.WorkflowDefinitionId, cancellationToken)
            ?? throw new WorkflowDefinitionNotFoundException(command.WorkflowDefinitionId);

        definition.Archive();

        var activeVersion = await versions.GetActiveForDefinitionAsync(definition.Id, cancellationToken);
        activeVersion?.Archive();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ArchiveWorkflowDefinitionResult(definition.Id);
    }
}
