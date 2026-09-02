using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Notifications.MarkNotificationRead;

public class MarkNotificationReadCommandHandler(
    INotificationRepository notifications, IUnitOfWork unitOfWork, ICurrentUserService currentUser, IDateTime dateTime)
    : ICommandHandler<MarkNotificationReadCommand, bool>
{
    public async Task<bool> Handle(MarkNotificationReadCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var notification = await notifications.GetByIdForRecipientAsync(command.NotificationId, userId, cancellationToken)
            ?? throw new NotificationNotFoundException(command.NotificationId);

        notification.MarkAsRead(dateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
