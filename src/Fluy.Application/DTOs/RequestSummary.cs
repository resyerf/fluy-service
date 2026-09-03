namespace Fluy.Application.DTOs;

public record RequestSummary(Guid Id, string Title, decimal? Amount, string Status, DateTimeOffset? SubmittedAt);
