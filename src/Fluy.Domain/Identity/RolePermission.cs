using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Identity;

public class RolePermission : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }

    private RolePermission()
    {
    }

    public static RolePermission Create(Guid tenantId, Guid roleId, Guid permissionId) => new()
    {
        TenantId = tenantId,
        RoleId = roleId,
        PermissionId = permissionId
    };
}
