using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.GetPermissionCatalog;

public class GetPermissionCatalogQueryHandler(IPermissionRepository permissions)
    : IQueryHandler<GetPermissionCatalogQuery, IReadOnlyCollection<PermissionDetail>>
{
    public Task<IReadOnlyCollection<PermissionDetail>> Handle(GetPermissionCatalogQuery query, CancellationToken cancellationToken) =>
        permissions.GetCatalogAsync(cancellationToken);
}
