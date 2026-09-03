using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Requests.GetRequestById;

public record GetRequestByIdQuery(Guid RequestId) : IQuery<RequestDetail>, IRequiresPermission
{
    public string PermissionCode => "request.view";
}
