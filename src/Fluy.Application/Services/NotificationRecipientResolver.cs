using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;

namespace Fluy.Application.Services;

/// <summary>
/// Resuelve a quién avisar cuando un Approval Pending queda asignado (CLAUDE.md §20). Espeja la
/// misma regla que <c>ApprovalAuthorizationService</c> usa para autorizar la decisión: si el paso exige un
/// rol concreto (<c>Approval.RequiredRoleId</c>, CODE.md §9.19/§9.20) se notifica a todos los
/// usuarios con ese rol; si no (fallback sin Workflow configurado), se notifica a todo el que tenga
/// el permiso `request.approve` en el tenant — el mismo universo que hoy ve "Aprobaciones pendientes".
/// </summary>
internal sealed class NotificationRecipientResolver(IUserRoleRepository userRoles) : INotificationRecipientResolver
{
    public Task<IReadOnlyCollection<Guid>> GetApproversAsync(Guid? requiredRoleId, CancellationToken cancellationToken) =>
        requiredRoleId is not null
            ? userRoles.GetUserIdsByRoleAsync(requiredRoleId.Value, cancellationToken)
            : userRoles.GetUserIdsWithPermissionAsync("request.approve", cancellationToken);
}
