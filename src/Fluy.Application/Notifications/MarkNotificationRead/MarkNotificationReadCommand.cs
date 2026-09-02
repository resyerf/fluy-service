using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Notifications.MarkNotificationRead;

public record MarkNotificationReadCommand(Guid NotificationId) : ICommand<bool>;
