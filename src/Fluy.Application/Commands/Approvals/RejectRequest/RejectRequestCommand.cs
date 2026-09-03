using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Approvals.RejectRequest;

public record RejectRequestCommand(Guid RequestId, string Comment) : ICommand<RejectRequestResult>, IRequiresPermission
{
    public string PermissionCode => "request.reject";
}
