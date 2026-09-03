using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Audit.GetAuditTrailForRequest;

public record GetAuditTrailForRequestQuery(Guid RequestId) : IQuery<IReadOnlyCollection<AuditEventDetail>>, IRequiresPermission
{
    public string PermissionCode => "audit.view";
}
