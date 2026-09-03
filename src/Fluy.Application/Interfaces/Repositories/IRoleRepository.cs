using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    void Add(Role role);
    void AddPermissions(IEnumerable<RolePermission> rolePermissions);
    Task<IReadOnlyCollection<RoleDetail>> GetAllWithPermissionsAsync(CancellationToken cancellationToken);
    Task<string?> GetNameAsync(Guid roleId, CancellationToken cancellationToken);
}
