using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Tenancy;

public class Branch : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = null!;

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private Branch()
    {
    }

    public static Branch Create(Guid tenantId, Guid companyId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre de la sede es obligatorio.", nameof(name));
        }

        return new Branch
        {
            TenantId = tenantId,
            CompanyId = companyId,
            Name = name.Trim()
        };
    }
}
