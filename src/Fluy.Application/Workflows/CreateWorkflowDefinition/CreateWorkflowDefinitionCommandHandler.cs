using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Workflows;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.CreateWorkflowDefinition;

public class CreateWorkflowDefinitionCommandHandler(
    IWorkflowDefinitionRepository definitions, IWorkflowVersionRepository versions, IUnitOfWork unitOfWork, ICurrentTenantService currentTenant)
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

        return new CreateWorkflowDefinitionResult(definition.Id, version.Id);
    }
}
