using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Rules;

/// <summary>
/// Rules Engine mínimo (CLAUDE.md §16, CODE.md §9.19): un único ejemplo concreto de la sección —
/// "IF amount > X THEN aprobación adicional de un rol específico" — no el intérprete de
/// condiciones AND/OR/rangos completo que describe CLAUDE.md (eso sigue en D04, sin resolver).
/// Una sola fila por tenant en este alcance mínimo: no hay todavía necesidad de reglas por sede,
/// categoría o rol del solicitante, ni de ordenar/priorizar varias reglas entre sí.
/// </summary>
public class ApprovalRule : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public decimal MinAmount { get; private set; }
    public Guid SecondApproverRoleId { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private ApprovalRule()
    {
    }

    public static ApprovalRule Create(Guid tenantId, decimal minAmount, Guid secondApproverRoleId)
    {
        if (minAmount < 0)
        {
            throw new ArgumentException("El monto mínimo no puede ser negativo.", nameof(minAmount));
        }

        return new ApprovalRule
        {
            TenantId = tenantId,
            MinAmount = minAmount,
            SecondApproverRoleId = secondApproverRoleId
        };
    }

    public void Update(decimal minAmount, Guid secondApproverRoleId)
    {
        if (minAmount < 0)
        {
            throw new ArgumentException("El monto mínimo no puede ser negativo.", nameof(minAmount));
        }

        MinAmount = minAmount;
        SecondApproverRoleId = secondApproverRoleId;
    }
}
