using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Entities;

/// <summary>
/// Audit Log real (CLAUDE.md §22), distinto de Business History (estado de Request/Approval) y
/// de Workflow History (WorkflowInstance). Append-only: nunca se modifica ni se borra físicamente
/// (CODE.md línea 331-332, igual que Request/Approval). Se crea inline desde cada Command Handler
/// que muta estado — mismo patrón pragmático que Notification (CODE.md §4.21: no hay todavía un
/// dispatcher de Domain Events real).
/// </summary>
public class AuditEvent : AggregateRoot, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public Guid? UserId { get; private set; }
    public string Action { get; private set; } = null!;
    public string EntityType { get; private set; } = null!;
    public Guid EntityId { get; private set; }
    public string? PreviousState { get; private set; }
    public string? NewState { get; private set; }
    public string? Metadata { get; private set; }
    public string? IpAddress { get; private set; }
    public string CorrelationId { get; private set; } = null!;
    public string? Reason { get; private set; }
    public string? Comment { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private AuditEvent()
    {
    }

    public static AuditEvent Create(
        Guid tenantId, Guid? userId, string action, string entityType, Guid entityId, DateTimeOffset now,
        string? previousState = null, string? newState = null, string? metadata = null, string? ipAddress = null,
        string correlationId = "", string? reason = null, string? comment = null)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            throw new ArgumentException("La acción es obligatoria.", nameof(action));
        }

        if (string.IsNullOrWhiteSpace(entityType))
        {
            throw new ArgumentException("El tipo de entidad es obligatorio.", nameof(entityType));
        }

        return new AuditEvent
        {
            TenantId = tenantId,
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            PreviousState = previousState,
            NewState = newState,
            Metadata = metadata,
            IpAddress = ipAddress,
            CorrelationId = correlationId,
            Reason = reason,
            Comment = comment,
            CreatedAt = now
        };
    }
}
