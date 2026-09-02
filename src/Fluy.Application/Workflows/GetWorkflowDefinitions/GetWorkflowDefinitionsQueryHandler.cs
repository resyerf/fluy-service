using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.GetWorkflowDefinitions;

public class GetWorkflowDefinitionsQueryHandler(IWorkflowDefinitionRepository definitions)
    : IQueryHandler<GetWorkflowDefinitionsQuery, IReadOnlyCollection<WorkflowDefinitionSummary>>
{
    public Task<IReadOnlyCollection<WorkflowDefinitionSummary>> Handle(
        GetWorkflowDefinitionsQuery query, CancellationToken cancellationToken) =>
        definitions.GetAllWithVersionsAsync(cancellationToken);
}
