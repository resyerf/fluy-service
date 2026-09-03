using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Approvals.ApproveRequest;

public record ApproveRequestCommand(Guid RequestId, string? Comment) : ICommand<ApproveRequestResult>, IRequiresPermission
{
    public string PermissionCode => "request.approve";
}
