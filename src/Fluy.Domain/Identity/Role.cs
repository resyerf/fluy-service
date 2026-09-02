using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Identity;

public class Role : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsSystemRole { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private Role()
    {
    }

    public static Role Create(Guid tenantId, string name, bool isSystemRole = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del rol es obligatorio.", nameof(name));
        }

        return new Role
        {
            TenantId = tenantId,
            Name = name.Trim(),
            IsSystemRole = isSystemRole
        };
    }
}
