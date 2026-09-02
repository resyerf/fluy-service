using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Requests.GetRequestById;

public class GetRequestByIdQueryHandler(IRequestRepository requests, IApprovalRepository approvals)
    : IQueryHandler<GetRequestByIdQuery, RequestDetail>
{
    public async Task<RequestDetail> Handle(GetRequestByIdQuery query, CancellationToken cancellationToken)
    {
        var request = await requests.GetByIdAsync(query.RequestId, cancellationToken)
            ?? throw new RequestNotFoundException(query.RequestId);

        var fields = await requests.GetFieldsAsync(request.Id, cancellationToken);
        var latestApproval = await approvals.GetLatestForRequestAsync(request.Id, cancellationToken);

        return new RequestDetail(
            request.Id, request.RequesterId, request.Title, request.Description, request.Amount,
            request.Status.ToString(), request.SubmittedAt, fields, latestApproval);
    }
}
