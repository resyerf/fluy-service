using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.GetPermissionCatalog;

public record GetPermissionCatalogQuery : IQuery<IReadOnlyCollection<PermissionDetail>>, IRequiresPermission
{
    public string PermissionCode => "roles.manage";
}

public record PermissionDetail(Guid Id, string Code, string Description);
