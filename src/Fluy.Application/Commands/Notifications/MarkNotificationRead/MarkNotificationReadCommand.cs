using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Notifications.MarkNotificationRead;

public record MarkNotificationReadCommand(Guid NotificationId) : ICommand<bool>;
