using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    void Add(User user);
    Task<IReadOnlyCollection<string>> GetRoleNamesAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<UserDetail>> GetAllWithRolesAsync(CancellationToken cancellationToken);

    void AddPasswordSetToken(PasswordSetToken token);
    Task<PasswordSetToken?> GetPasswordSetTokenByHashAsync(string tokenHash, CancellationToken cancellationToken);
}
