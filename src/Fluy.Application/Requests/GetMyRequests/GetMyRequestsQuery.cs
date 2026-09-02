using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Requests.GetMyRequests;

/// <summary>BranchId nulo = sin filtrar por sede (tenant sin sedes configuradas, o usuario sin sede activa elegida — CODE.md §9.25).</summary>
public record GetMyRequestsQuery(Guid? BranchId = null) : IQuery<IReadOnlyCollection<RequestSummary>>, IRequiresPermission
{
    public string PermissionCode => "request.view";
}

public record RequestSummary(Guid Id, string Title, decimal? Amount, string Status, DateTimeOffset? SubmittedAt);
