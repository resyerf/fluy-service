using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Identity.GetPermissionCatalog;

public record GetPermissionCatalogQuery : IQuery<IReadOnlyCollection<PermissionDetail>>, IRequiresPermission
{
    public string PermissionCode => "roles.manage";
}
