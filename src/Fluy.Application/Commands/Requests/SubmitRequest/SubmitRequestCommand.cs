using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Requests.SubmitRequest;

public record SubmitRequestCommand(Guid RequestId) : ICommand<SubmitRequestResult>, IRequiresPermission
{
    public string PermissionCode => "request.create";
}
