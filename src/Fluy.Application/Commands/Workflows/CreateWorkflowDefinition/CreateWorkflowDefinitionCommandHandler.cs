using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Workflows.CreateWorkflowDefinition;

public class CreateWorkflowDefinitionCommandHandler(
    IWorkflowDefinitionRepository definitions, IWorkflowVersionRepository versions, IUnitOfWork unitOfWork,
    ICurrentTenantService currentTenant, IUsageTracker usageTracker)
    : ICommandHandler<CreateWorkflowDefinitionCommand, CreateWorkflowDefinitionResult>
{
    public async Task<CreateWorkflowDefinitionResult> Handle(CreateWorkflowDefinitionCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var definition = WorkflowDefinition.Create(tenantId, command.Name, command.Description);
        var version = WorkflowVersion.CreateDraft(tenantId, definition.Id, versionNumber: 1);

        definitions.Add(definition);
        versions.Add(version);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await usageTracker.IncrementAsync(tenantId, "max.workflows", 1, cancellationToken);

        return new CreateWorkflowDefinitionResult(definition.Id, version.Id);
    }
}
