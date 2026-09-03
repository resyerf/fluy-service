using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Notifications.ArchiveNotification;

public record ArchiveNotificationCommand(Guid NotificationId) : ICommand<bool>;
