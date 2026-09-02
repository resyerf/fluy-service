using Fluy.Application.Notifications.GetMyNotifications;
using Fluy.Domain.Notifications;

namespace Fluy.Application.Common.Interfaces.Repositories;

public interface INotificationRepository
{
    void Add(Notification notification);
    Task<Notification?> GetByIdForRecipientAsync(Guid id, Guid recipientUserId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Notification>> GetUnreadForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<NotificationsResult> GetMyNotificationsAsync(Guid userId, NotificationFilter filter, CancellationToken cancellationToken);
}
