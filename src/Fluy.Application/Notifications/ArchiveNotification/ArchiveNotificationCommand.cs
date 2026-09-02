using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Notifications.ArchiveNotification;

public record ArchiveNotificationCommand(Guid NotificationId) : ICommand<bool>;
