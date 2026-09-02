using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Tenancy;

public class Department : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid BranchId { get; private set; }
    public string Name { get; private set; } = null!;

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private Department()
    {
    }

    public static Department Create(Guid tenantId, Guid branchId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del departamento es obligatorio.", nameof(name));
        }

        return new Department
        {
            TenantId = tenantId,
            BranchId = branchId,
            Name = name.Trim()
        };
    }
}
