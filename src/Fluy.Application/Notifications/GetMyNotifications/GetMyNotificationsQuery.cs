using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Notifications.GetMyNotifications;

public enum NotificationFilter
{
    All,
    Unread,
    Archived
}

public record GetMyNotificationsQuery(NotificationFilter Filter = NotificationFilter.All)
    : IQuery<NotificationsResult>;

public record NotificationSummary(
    Guid Id, string Type, string Title, string Message, Guid? RequestId,
    string? ActorName, bool IsRead, bool IsArchived, DateTimeOffset CreatedAt);

public record NotificationsResult(
    IReadOnlyCollection<NotificationSummary> Items, int TotalCount, int UnreadCount, int ArchivedCount);
