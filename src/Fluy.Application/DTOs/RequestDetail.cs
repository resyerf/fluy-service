namespace Fluy.Application.DTOs;

public record RequestDetail(
    Guid Id, Guid RequesterId, string Title, string Description, decimal? Amount,
    string Status, DateTimeOffset? SubmittedAt, IReadOnlyCollection<RequestFieldDetail> Fields,
    LatestApprovalDetail? LatestApproval);
