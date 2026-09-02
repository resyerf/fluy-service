using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Notifications;

/// <summary>
/// Notificación in-app dirigida a un usuario puntual (CLAUDE.md §20/§25). Se crea desde los mismos
/// Command Handlers que ya mutan Request/Approval (ApproveRequest, RejectRequest, RequestCorrection,
/// SubmitRequest) — no existe todavía un event bus/domain events despachado (CODE.md §11.33,
/// "pendiente"), así que esto sigue el mismo patrón pragmático que el resto del Approval Engine
/// mínimo: el handler que causa el hecho crea directamente la fila.
/// </summary>
public class Notification : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid RecipientUserId { get; private set; }
    public Guid? ActorUserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public Guid? RequestId { get; private set; }
    public bool IsRead { get; private set; }
    public bool IsArchived { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private Notification()
    {
    }

    public static Notification Create(
        Guid tenantId, Guid recipientUserId, NotificationType type, string title, string message,
        Guid? actorUserId = null, Guid? requestId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("El título de la notificación es obligatorio.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("El mensaje de la notificación es obligatorio.", nameof(message));
        }

        return new Notification
        {
            TenantId = tenantId,
            RecipientUserId = recipientUserId,
            ActorUserId = actorUserId,
            Type = type,
            Title = title.Trim(),
            Message = message.Trim(),
            RequestId = requestId,
            IsRead = false,
            IsArchived = false
        };
    }

    public void MarkAsRead(DateTimeOffset now)
    {
        if (IsRead)
        {
            return;
        }

        IsRead = true;
        ReadAt = now;
    }

    public void Archive() => IsArchived = true;
}
