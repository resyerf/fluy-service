namespace Fluy.Application.DTOs;

public record NotificationsResult(
    IReadOnlyCollection<NotificationSummary> Items, int TotalCount, int UnreadCount, int ArchivedCount);
