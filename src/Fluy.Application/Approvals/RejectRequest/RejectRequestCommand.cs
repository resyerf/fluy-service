using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Approvals.RejectRequest;

public record RejectRequestCommand(Guid RequestId, string Comment) : ICommand<RejectRequestResult>, IRequiresPermission
{
    public string PermissionCode => "request.reject";
}

public record RejectRequestResult(Guid RequestId, string Status);
