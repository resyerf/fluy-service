using Fluy.Application.Interfaces.Repositories;
using Fluy.Application.DTOs;
using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Fluy.Infrastructure.Persistence.Context;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(ApplicationDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) =>
        db.Users.AnyAsync(u => u.Id == id, cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken) =>
        db.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public void Add(User user) => db.Users.Add(user);

    public async Task<IReadOnlyCollection<string>> GetRoleNamesAsync(Guid userId, CancellationToken cancellationToken) =>
        await (
                from userRole in db.UserRoles
                join role in db.Roles on userRole.RoleId equals role.Id
                where userRole.UserId == userId
                select role.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<UserDetail>> GetAllWithRolesAsync(CancellationToken cancellationToken)
    {
        var users = await db.Users.AsNoTracking()
            .Select(u => new { u.Id, u.Email, u.FullName, Status = u.Status.ToString() })
            .ToListAsync(cancellationToken);

        var roleAssignments = await db.UserRoles.AsNoTracking()
            .Join(db.Roles.AsNoTracking(), ur => ur.RoleId, r => r.Id, (ur, r) => new
            {
                ur.Id,
                ur.UserId,
                ur.RoleId,
                RoleName = r.Name,
                ur.BranchId,
                ur.DepartmentId
            })
            .ToListAsync(cancellationToken);

        return users
            .Select(u => new UserDetail(
                u.Id,
                u.Email,
                u.FullName,
                u.Status,
                roleAssignments
                    .Where(ra => ra.UserId == u.Id)
                    .Select(ra => new UserRoleDetail(ra.Id, ra.RoleId, ra.RoleName, ra.BranchId, ra.DepartmentId))
                    .ToList()))
            .ToList();
    }

    public void AddPasswordSetToken(PasswordSetToken token) => db.PasswordSetTokens.Add(token);

    public Task<PasswordSetToken?> GetPasswordSetTokenByHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        db.PasswordSetTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
}
