using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Approvals.ApproveRequest;

public record ApproveRequestCommand(Guid RequestId, string? Comment) : ICommand<ApproveRequestResult>, IRequiresPermission
{
    public string PermissionCode => "request.approve";
}

public record ApproveRequestResult(Guid RequestId, string Status);
