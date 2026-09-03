using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Notifications.MarkAllNotificationsRead;

public record MarkAllNotificationsReadCommand : ICommand<int>;
