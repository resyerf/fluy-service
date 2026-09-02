using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Requests.GetRequestById;

public record GetRequestByIdQuery(Guid RequestId) : IQuery<RequestDetail>, IRequiresPermission
{
    public string PermissionCode => "request.view";
}

public record RequestDetail(
    Guid Id, Guid RequesterId, string Title, string Description, decimal? Amount,
    string Status, DateTimeOffset? SubmittedAt, IReadOnlyCollection<RequestFieldDetail> Fields,
    LatestApprovalDetail? LatestApproval);

public record RequestFieldDetail(string Key, string Value);

/// <summary>La última decisión (o Pending) — suficiente para el alcance actual (un solo paso, o dos
/// si aplica la ApprovalRule de CODE.md §9.19); con un Workflow multi-paso real esto pasaría a ser
/// una lista de pasos, no un único "latest". `RequiredRoleName` es null en tier 1 (cualquier
/// aprobador) y tiene valor en tier 2 (solo ese rol puede decidir).</summary>
public record LatestApprovalDetail(string Status, string? Comment, DateTimeOffset? DecidedAt, int Tier, string? RequiredRoleName);
