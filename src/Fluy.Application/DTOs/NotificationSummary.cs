namespace Fluy.Application.DTOs;

public record NotificationSummary(
    Guid Id, string Type, string Title, string Message, Guid? RequestId,
    string? ActorName, bool IsRead, bool IsArchived, DateTimeOffset CreatedAt);
