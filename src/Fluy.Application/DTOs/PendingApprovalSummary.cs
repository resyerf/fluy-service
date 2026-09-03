namespace Fluy.Application.DTOs;

public record PendingApprovalSummary(
    Guid ApprovalId, Guid RequestId, string RequestTitle, decimal? Amount, string RequesterEmail,
    DateTimeOffset? SubmittedAt, int Tier, string? RequiredRoleName);
