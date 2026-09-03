using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Notifications.GetMyNotifications;

public record GetMyNotificationsQuery(NotificationFilter Filter = NotificationFilter.All)
    : IQuery<NotificationsResult>;
