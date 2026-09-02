using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Requests.CreateRequest;

public record CreateRequestFieldInput(string Key, string Value);

/// <summary>
/// Primer Command de negocio real del sistema (CLAUDE.md §3, paso 1-2) — y el primero que declara
/// IRequiresPermission, cableando TenantAuthorizationBehavior al Dispatcher (CODE.md §10-D19).
/// </summary>
public record CreateRequestCommand(
    string Title, string Description, decimal? Amount, IReadOnlyCollection<CreateRequestFieldInput>? Fields, Guid? BranchId = null)
    : ICommand<CreateRequestResult>, IRequiresPermission
{
    public string PermissionCode => "request.create";
}
