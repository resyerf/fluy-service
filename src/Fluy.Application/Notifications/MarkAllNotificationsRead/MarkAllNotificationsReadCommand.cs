using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Notifications.MarkAllNotificationsRead;

public record MarkAllNotificationsReadCommand : ICommand<int>;
