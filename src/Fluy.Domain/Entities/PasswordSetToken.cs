using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Entities;

/// <summary>
/// Token de un solo uso para que un usuario recién creado (típicamente el master de un tenant
/// aprovisionado, CODE.md §9.5) defina su contraseña inicial. Nunca se almacena en texto plano —
/// solo su hash (SHA-256, ver Fluy.Infrastructure/Identity/TokenHasher); el valor crudo se genera
/// una vez y se devuelve al momento de crearlo, no se puede recuperar después.
/// </summary>
public class PasswordSetToken : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? UsedAt { get; private set; }

    private PasswordSetToken()
    {
    }

    public static PasswordSetToken Create(Guid tenantId, Guid userId, string tokenHash, DateTimeOffset expiresAt) => new()
    {
        TenantId = tenantId,
        UserId = userId,
        TokenHash = tokenHash,
        ExpiresAt = expiresAt
    };

    public bool IsValid(DateTimeOffset now) => UsedAt is null && now <= ExpiresAt;

    public void MarkUsed(DateTimeOffset usedAt) => UsedAt = usedAt;
}
