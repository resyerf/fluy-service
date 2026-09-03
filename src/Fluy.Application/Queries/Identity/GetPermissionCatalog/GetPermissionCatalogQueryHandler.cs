using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Identity.GetPermissionCatalog;

public class GetPermissionCatalogQueryHandler(IPermissionRepository permissions)
    : IQueryHandler<GetPermissionCatalogQuery, IReadOnlyCollection<PermissionDetail>>
{
    public Task<IReadOnlyCollection<PermissionDetail>> Handle(GetPermissionCatalogQuery query, CancellationToken cancellationToken) =>
        permissions.GetCatalogAsync(cancellationToken);
}
