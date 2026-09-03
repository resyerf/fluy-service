using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Queries.Audit.GetAuditTrailForRequest;

public class GetAuditTrailForRequestQueryHandler(IAuditEventRepository auditEvents)
    : IQueryHandler<GetAuditTrailForRequestQuery, IReadOnlyCollection<AuditEventDetail>>
{
    public Task<IReadOnlyCollection<AuditEventDetail>> Handle(GetAuditTrailForRequestQuery query, CancellationToken cancellationToken) =>
        auditEvents.GetByEntityAsync(nameof(Fluy.Domain.Entities.Request), query.RequestId, cancellationToken);
}
