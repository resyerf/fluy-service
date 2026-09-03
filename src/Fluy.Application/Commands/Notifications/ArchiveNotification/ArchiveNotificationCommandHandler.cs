using Fluy.Application.Common.Exceptions;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Notifications.ArchiveNotification;

public class ArchiveNotificationCommandHandler(INotificationRepository notifications, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : ICommandHandler<ArchiveNotificationCommand, bool>
{
    public async Task<bool> Handle(ArchiveNotificationCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var notification = await notifications.GetByIdForRecipientAsync(command.NotificationId, userId, cancellationToken)
            ?? throw new NotificationNotFoundException(command.NotificationId);

        notification.Archive();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
