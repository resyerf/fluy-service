using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Notifications.MarkAllNotificationsRead;

public class MarkAllNotificationsReadCommandHandler(
    INotificationRepository notifications, IUnitOfWork unitOfWork, ICurrentUserService currentUser, IDateTime dateTime)
    : ICommandHandler<MarkAllNotificationsReadCommand, int>
{
    public async Task<int> Handle(MarkAllNotificationsReadCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var unread = await notifications.GetUnreadForUserAsync(userId, cancellationToken);

        var now = dateTime.UtcNow;
        foreach (var notification in unread)
        {
            notification.MarkAsRead(now);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return unread.Count;
    }
}
