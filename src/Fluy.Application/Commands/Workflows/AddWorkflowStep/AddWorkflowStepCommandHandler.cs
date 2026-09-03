using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Fluy.Domain.Enums;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Workflows.AddWorkflowStep;

public class AddWorkflowStepCommandHandler(
    IWorkflowVersionRepository versions, IRoleRepository roles, IUnitOfWork unitOfWork, ICurrentTenantService currentTenant)
    : ICommandHandler<AddWorkflowStepCommand, AddWorkflowStepResult>
{
    public async Task<AddWorkflowStepResult> Handle(AddWorkflowStepCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var version = await versions.GetByIdAsync(command.WorkflowVersionId, cancellationToken)
            ?? throw new WorkflowVersionNotFoundException(command.WorkflowVersionId);

        if (version.Status != WorkflowVersionStatus.Draft)
        {
            throw new InvalidWorkflowStateException("Solo se pueden agregar pasos a una versión en Draft.");
        }

        var roleExists = await roles.ExistsAsync(command.ApproverRoleId, cancellationToken);
        if (!roleExists)
        {
            throw new RoleNotFoundException(command.ApproverRoleId);
        }

        var order = await versions.CountStepsAsync(version.Id, cancellationToken);

        var step = WorkflowStep.Create(tenantId, version.Id, command.Name, command.ApproverRoleId, order);
        versions.AddStep(step);

        if (version.InitialStepId is null)
        {
            version.SetInitialStep(step.Id);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddWorkflowStepResult(step.Id);
    }
}
