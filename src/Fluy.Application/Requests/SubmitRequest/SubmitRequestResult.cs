namespace Fluy.Application.Requests.SubmitRequest;

public record SubmitRequestResult(Guid RequestId, string Status, DateTimeOffset SubmittedAt);
