using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Notifications.GetMyNotifications;

/// <summary>
/// Bandeja de notificaciones del usuario actual (CLAUDE.md §20/§24). Las tres tabs de la UI (Todas /
/// No leídas / Archivadas) comparten un único query: se calculan los tres contadores siempre —
/// necesarios para las badges de las tabs — y se filtra la lista devuelta según <see cref="GetMyNotificationsQuery.Filter"/>.
/// </summary>
public class GetMyNotificationsQueryHandler(INotificationRepository notifications, ICurrentUserService currentUser)
    : IQueryHandler<GetMyNotificationsQuery, NotificationsResult>
{
    public Task<NotificationsResult> Handle(GetMyNotificationsQuery query, CancellationToken cancellationToken) =>
        notifications.GetMyNotificationsAsync(currentUser.UserId!.Value, query.Filter, cancellationToken);
}
