using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Workflows.GetWorkflowVersionDetail;

public class GetWorkflowVersionDetailQueryHandler(IWorkflowVersionRepository versions)
    : IQueryHandler<GetWorkflowVersionDetailQuery, WorkflowVersionDetail>
{
    public Task<WorkflowVersionDetail> Handle(GetWorkflowVersionDetailQuery query, CancellationToken cancellationToken) =>
        versions.GetDetailAsync(query.WorkflowVersionId, cancellationToken);
}
