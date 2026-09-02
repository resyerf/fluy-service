using Fluy.Domain.Identity;

namespace Fluy.Application.Common.Interfaces.Repositories;

public interface IUserRoleRepository
{
    void Add(UserRole userRole);
    Task<IReadOnlyCollection<Guid?>> GetBranchIdsForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> HasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Guid>> GetUserIdsByRoleAsync(Guid roleId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Guid>> GetUserIdsWithPermissionAsync(string permissionCode, CancellationToken cancellationToken);
}
