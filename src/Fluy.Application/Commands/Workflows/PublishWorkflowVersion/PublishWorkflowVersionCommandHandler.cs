using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Workflows.PublishWorkflowVersion;

/// <summary>
/// Publica una versión y, para mantener el alcance de "un solo workflow activo por tenant"
/// (CODE.md §9.20, mismo límite que tenía <c>ApprovalRule</c>), archiva cualquier otra definición
/// que estuviera Published (y su versión Active) antes de activar esta.
/// </summary>
public class PublishWorkflowVersionCommandHandler(
    IWorkflowVersionRepository versions, IWorkflowDefinitionRepository definitions, IUnitOfWork unitOfWork,
    ICurrentTenantService currentTenant, IDateTime dateTime)
    : ICommandHandler<PublishWorkflowVersionCommand, PublishWorkflowVersionResult>
{
    public async Task<PublishWorkflowVersionResult> Handle(PublishWorkflowVersionCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var version = await versions.GetByIdAsync(command.WorkflowVersionId, cancellationToken)
            ?? throw new WorkflowVersionNotFoundException(command.WorkflowVersionId);

        var definition = await definitions.GetByIdAsync(version.WorkflowDefinitionId, cancellationToken)
            ?? throw new WorkflowDefinitionNotFoundException(version.WorkflowDefinitionId);

        var steps = await versions.GetStepsAsync(version.Id, cancellationToken);
        var transitions = await versions.GetTransitionsAsync(version.Id, cancellationToken);

        var now = dateTime.UtcNow;

        try
        {
            version.Publish(steps, transitions, now);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidWorkflowStateException(ex.Message);
        }

        var otherPublishedDefinitions = await definitions.GetOtherPublishedAsync(tenantId, definition.Id, cancellationToken);

        foreach (var other in otherPublishedDefinitions)
        {
            other.Archive();

            var otherActiveVersion = await versions.GetActiveForDefinitionAsync(other.Id, cancellationToken);
            otherActiveVersion?.Archive();
        }

        definition.MarkPublished();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new PublishWorkflowVersionResult(definition.Id, version.Id);
    }
}
