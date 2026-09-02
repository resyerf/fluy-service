using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Tenancy;

public class Company : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? LegalIdentifier { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private Company()
    {
    }

    public static Company Create(Guid tenantId, string name, string? legalIdentifier = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre de la empresa es obligatorio.", nameof(name));
        }

        return new Company
        {
            TenantId = tenantId,
            Name = name.Trim(),
            LegalIdentifier = legalIdentifier?.Trim()
        };
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre de la empresa es obligatorio.", nameof(name));
        }

        Name = name.Trim();
    }
}
