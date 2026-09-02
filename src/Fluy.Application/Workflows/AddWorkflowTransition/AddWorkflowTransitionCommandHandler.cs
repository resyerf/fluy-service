using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Workflows;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.AddWorkflowTransition;

public class AddWorkflowTransitionCommandHandler(IWorkflowVersionRepository versions, IUnitOfWork unitOfWork, ICurrentTenantService currentTenant)
    : ICommandHandler<AddWorkflowTransitionCommand, AddWorkflowTransitionResult>
{
    public async Task<AddWorkflowTransitionResult> Handle(AddWorkflowTransitionCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var version = await versions.GetByIdAsync(command.WorkflowVersionId, cancellationToken)
            ?? throw new WorkflowVersionNotFoundException(command.WorkflowVersionId);

        if (version.Status != WorkflowVersionStatus.Draft)
        {
            throw new InvalidWorkflowStateException("Solo se pueden agregar transiciones a una versión en Draft.");
        }

        var fromStepExists = await versions.StepExistsAsync(command.FromStepId, version.Id, cancellationToken);
        if (!fromStepExists)
        {
            throw new WorkflowStepNotFoundException(command.FromStepId);
        }

        if (command.ToStepId is not null)
        {
            var toStepExists = await versions.StepExistsAsync(command.ToStepId.Value, version.Id, cancellationToken);
            if (!toStepExists)
            {
                throw new WorkflowStepNotFoundException(command.ToStepId.Value);
            }
        }

        WorkflowConditionOperator? conditionOperator = command.ConditionOperator is null
            ? null
            : Enum.Parse<WorkflowConditionOperator>(command.ConditionOperator);

        var transition = WorkflowTransition.Create(
            tenantId, version.Id, command.FromStepId, command.ToStepId,
            command.ConditionField, conditionOperator, command.ConditionValue, command.Order);

        versions.AddTransition(transition);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddWorkflowTransitionResult(transition.Id);
    }
}
