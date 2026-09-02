using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Identity;

public class User : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public string Email { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserStatus Status { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private User()
    {
    }

    public static User Create(Guid tenantId, string email, string fullName, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El email del usuario es obligatorio.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("El nombre del usuario es obligatorio.", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("El hash de contraseña es obligatorio.", nameof(passwordHash));
        }

        return new User
        {
            TenantId = tenantId,
            Email = email.Trim().ToLowerInvariant(),
            FullName = fullName.Trim(),
            PasswordHash = passwordHash,
            Status = UserStatus.Active
        };
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            throw new ArgumentException("El hash de contraseña es obligatorio.", nameof(newPasswordHash));
        }

        PasswordHash = newPasswordHash;
    }

    public void Disable() => Status = UserStatus.Disabled;

    public void Activate() => Status = UserStatus.Active;
}
