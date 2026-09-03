using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Entities;

/// <summary>
/// Entidad independiente, no una colección navegable desde Request — mismo patrón que
/// RolePermission/UserRole en este codebase: se crea y consulta directamente vía su propio
/// DbSet, no a través de Request.Fields, para no complicar el mapeo de EF Core con una colección
/// respaldada por campo privado sin necesidad real todavía.
/// </summary>
public class RequestField : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid RequestId { get; private set; }
    public string Key { get; private set; } = null!;
    public string Value { get; private set; } = null!;

    private RequestField()
    {
    }

    public static RequestField Create(Guid tenantId, Guid requestId, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("La clave del campo es obligatoria.", nameof(key));
        }

        return new RequestField
        {
            TenantId = tenantId,
            RequestId = requestId,
            Key = key.Trim(),
            Value = value?.Trim() ?? string.Empty
        };
    }
}
