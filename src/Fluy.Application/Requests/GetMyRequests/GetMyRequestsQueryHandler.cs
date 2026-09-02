using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Requests.GetMyRequests;

/// <summary>
/// "Mis solicitudes" (CLAUDE.md §24, dashboard del Solicitante). Filtrado por RequesterId además
/// del Global Query Filter de tenant — un usuario ve sus propias solicitudes, no las de todo el
/// tenant (eso sería un dashboard de aprobador/admin, todavía no implementado).
/// </summary>
public class GetMyRequestsQueryHandler(IRequestRepository requests, ICurrentUserService currentUser)
    : IQueryHandler<GetMyRequestsQuery, IReadOnlyCollection<RequestSummary>>
{
    public Task<IReadOnlyCollection<RequestSummary>> Handle(GetMyRequestsQuery query, CancellationToken cancellationToken) =>
        requests.GetMineAsync(currentUser.UserId!.Value, query.BranchId, cancellationToken);
}
