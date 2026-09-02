using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Identity;

/// <summary>
/// Asigna un rol a un usuario. BranchId/DepartmentId nulos = alcance a todo el tenant;
/// con valor = el rol solo aplica en esa sede/departamento (CLAUDE.md §7, CODE.md §4.10).
/// </summary>
public class UserRole : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? DepartmentId { get; private set; }

    private UserRole()
    {
    }

    public static UserRole Create(Guid tenantId, Guid userId, Guid roleId, Guid? branchId = null, Guid? departmentId = null)
    {
        if (departmentId is not null && branchId is null)
        {
            throw new ArgumentException("Un alcance de departamento requiere especificar también la sede.", nameof(branchId));
        }

        return new UserRole
        {
            TenantId = tenantId,
            UserId = userId,
            RoleId = roleId,
            BranchId = branchId,
            DepartmentId = departmentId
        };
    }
}
