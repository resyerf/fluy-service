using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Workflows.GetWorkflowVersionDetail;

public class GetWorkflowVersionDetailQueryHandler(IWorkflowVersionRepository versions)
    : IQueryHandler<GetWorkflowVersionDetailQuery, WorkflowVersionDetail>
{
    public Task<WorkflowVersionDetail> Handle(GetWorkflowVersionDetailQuery query, CancellationToken cancellationToken) =>
        versions.GetDetailAsync(query.WorkflowVersionId, cancellationToken);
}
