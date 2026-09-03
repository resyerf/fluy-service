using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Workflows.GetWorkflowDefinitions;

public class GetWorkflowDefinitionsQueryHandler(IWorkflowDefinitionRepository definitions)
    : IQueryHandler<GetWorkflowDefinitionsQuery, IReadOnlyCollection<WorkflowDefinitionSummary>>
{
    public Task<IReadOnlyCollection<WorkflowDefinitionSummary>> Handle(
        GetWorkflowDefinitionsQuery query, CancellationToken cancellationToken) =>
        definitions.GetAllWithVersionsAsync(cancellationToken);
}
