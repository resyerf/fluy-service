using Fluy.Application.Identity.GetPermissionCatalog;
using Fluy.Domain.Identity;

namespace Fluy.Application.Common.Interfaces.Repositories;

public interface IPermissionRepository
{
    Task<IReadOnlyCollection<PermissionDetail>> GetCatalogAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Permission>> GetByCodesAsync(IReadOnlyCollection<string> codes, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Permission>> GetAllAsync(CancellationToken cancellationToken);
}
