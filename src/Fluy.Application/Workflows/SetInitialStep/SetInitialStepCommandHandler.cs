using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Workflows;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.SetInitialStep;

public class SetInitialStepCommandHandler(IWorkflowVersionRepository versions, IUnitOfWork unitOfWork)
    : ICommandHandler<SetInitialStepCommand, SetInitialStepResult>
{
    public async Task<SetInitialStepResult> Handle(SetInitialStepCommand command, CancellationToken cancellationToken)
    {
        var version = await versions.GetByIdAsync(command.WorkflowVersionId, cancellationToken)
            ?? throw new WorkflowVersionNotFoundException(command.WorkflowVersionId);

        if (version.Status != WorkflowVersionStatus.Draft)
        {
            throw new InvalidWorkflowStateException("Solo se puede cambiar el paso inicial de una versión en Draft.");
        }

        var stepExists = await versions.StepExistsAsync(command.StepId, version.Id, cancellationToken);
        if (!stepExists)
        {
            throw new WorkflowStepNotFoundException(command.StepId);
        }

        version.SetInitialStep(command.StepId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SetInitialStepResult(version.Id, command.StepId);
    }
}
