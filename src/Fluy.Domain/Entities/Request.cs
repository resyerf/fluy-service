using Fluy.Domain.Common;
using Fluy.SharedKernel;
using Fluy.Domain.Enums;

namespace Fluy.Domain.Entities;

public class Request : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid RequesterId { get; private set; }
    public Guid? BranchId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal? Amount { get; private set; }
    public RequestStatus Status { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private Request()
    {
    }

    /// <summary>
    /// BranchId es opcional (CODE.md §9.25): tenants que todavía no configuraron sedes en Organization
    /// siguen pudiendo crear solicitudes sin sede — el filtrado por sede activa en "Mis solicitudes"/
    /// "Aprobaciones pendientes" solo aplica cuando el frontend efectivamente tiene una sede activa
    /// que enviar, nunca es un requisito nuevo obligatorio que pueda romper tenants existentes.
    /// </summary>
    public static Request Create(
        Guid tenantId, Guid requesterId, string title, string description, decimal? amount, Guid? branchId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("El título de la solicitud es obligatorio.", nameof(title));
        }

        if (amount is < 0)
        {
            throw new ArgumentException("El monto no puede ser negativo.", nameof(amount));
        }

        return new Request
        {
            TenantId = tenantId,
            RequesterId = requesterId,
            BranchId = branchId,
            Title = title.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Amount = amount,
            Status = RequestStatus.Draft
        };
    }

    /// <summary>Draft → Submitted (primer envío) o ReturnedForCorrection → Submitted (reenvío tras corrección).</summary>
    public void Submit(DateTimeOffset now)
    {
        if (Status is not (RequestStatus.Draft or RequestStatus.ReturnedForCorrection))
        {
            throw new InvalidOperationException($"Solo una solicitud en Draft o ReturnedForCorrection puede enviarse (estado actual: {Status}).");
        }

        Status = RequestStatus.Submitted;
        SubmittedAt = now;
    }

    /// <summary>
    /// Aprobación única (Approval Engine mínimo, CODE.md §9.16): al no existir todavía un Workflow
    /// Engine con pasos múltiples, un único "sí" cierra la solicitud — no hay un paso posterior
    /// (contabilidad, etc.) que modelar todavía.
    /// </summary>
    public void Complete()
    {
        EnsureSubmitted();
        Status = RequestStatus.Completed;
    }

    public void Reject()
    {
        EnsureSubmitted();
        Status = RequestStatus.Rejected;
    }

    public void ReturnForCorrection()
    {
        EnsureSubmitted();
        Status = RequestStatus.ReturnedForCorrection;
    }

    private void EnsureSubmitted()
    {
        if (Status != RequestStatus.Submitted)
        {
            throw new InvalidOperationException($"Solo una solicitud en Submitted puede resolverse (estado actual: {Status}).");
        }
    }
}
