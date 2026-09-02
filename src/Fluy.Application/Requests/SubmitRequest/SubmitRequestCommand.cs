using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Requests.SubmitRequest;

public record SubmitRequestCommand(Guid RequestId) : ICommand<SubmitRequestResult>, IRequiresPermission
{
    public string PermissionCode => "request.create";
}
