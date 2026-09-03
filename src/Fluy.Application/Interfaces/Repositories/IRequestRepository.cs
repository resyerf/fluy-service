using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IRequestRepository
{
    Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(Request request);
    void AddFields(IEnumerable<RequestField> fields);
    Task<IReadOnlyCollection<RequestFieldDetail>> GetFieldsAsync(Guid requestId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<RequestSummary>> GetMineAsync(Guid requesterId, Guid? branchId, CancellationToken cancellationToken);
}
