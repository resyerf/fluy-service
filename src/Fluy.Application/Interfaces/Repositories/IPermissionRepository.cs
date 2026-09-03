using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IPermissionRepository
{
    Task<IReadOnlyCollection<PermissionDetail>> GetCatalogAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Permission>> GetByCodesAsync(IReadOnlyCollection<string> codes, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Permission>> GetAllAsync(CancellationToken cancellationToken);
}
