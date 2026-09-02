using Fluy.Application.Requests.GetMyRequests;
using Fluy.Application.Requests.GetRequestById;
using Fluy.Domain.Requests;

namespace Fluy.Application.Common.Interfaces.Repositories;

public interface IRequestRepository
{
    Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(Request request);
    void AddFields(IEnumerable<RequestField> fields);
    Task<IReadOnlyCollection<RequestFieldDetail>> GetFieldsAsync(Guid requestId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<RequestSummary>> GetMineAsync(Guid requesterId, Guid? branchId, CancellationToken cancellationToken);
}
