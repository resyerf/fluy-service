namespace Fluy.Application.DTOs;

public record SubmitRequestResult(Guid RequestId, string Status, DateTimeOffset SubmittedAt);
