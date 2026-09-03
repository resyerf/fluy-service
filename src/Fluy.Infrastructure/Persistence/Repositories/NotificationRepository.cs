using Fluy.Application.Interfaces.Repositories;
using Fluy.Application.DTOs;
using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Fluy.Infrastructure.Persistence.Context;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class NotificationRepository(ApplicationDbContext db) : INotificationRepository
{
    public void Add(Notification notification) => db.Notifications.Add(notification);

    public Task<Notification?> GetByIdForRecipientAsync(Guid id, Guid recipientUserId, CancellationToken cancellationToken) =>
        db.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.RecipientUserId == recipientUserId, cancellationToken);

    public async Task<IReadOnlyCollection<Notification>> GetUnreadForUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await db.Notifications
            .Where(n => n.RecipientUserId == userId && !n.IsArchived && !n.IsRead)
            .ToListAsync(cancellationToken);

    public async Task<NotificationsResult> GetMyNotificationsAsync(
        Guid userId, NotificationFilter filter, CancellationToken cancellationToken)
    {
        var mine = db.Notifications.AsNoTracking().Where(n => n.RecipientUserId == userId);

        var totalCount = await mine.CountAsync(n => !n.IsArchived, cancellationToken);
        var unreadCount = await mine.CountAsync(n => !n.IsArchived && !n.IsRead, cancellationToken);
        var archivedCount = await mine.CountAsync(n => n.IsArchived, cancellationToken);

        var filtered = filter switch
        {
            NotificationFilter.Unread => mine.Where(n => !n.IsArchived && !n.IsRead),
            NotificationFilter.Archived => mine.Where(n => n.IsArchived),
            _ => mine.Where(n => !n.IsArchived)
        };

        var items = await (
                from notification in filtered
                join actor in db.Users on notification.ActorUserId equals actor.Id into actors
                from actor in actors.DefaultIfEmpty()
                orderby notification.CreatedAt descending
                select new NotificationSummary(
                    notification.Id, notification.Type.ToString(), notification.Title, notification.Message,
                    notification.RequestId, actor != null ? actor.FullName : null,
                    notification.IsRead, notification.IsArchived, notification.CreatedAt))
            .Take(50)
            .ToListAsync(cancellationToken);

        return new NotificationsResult(items, totalCount, unreadCount, archivedCount);
    }
}
