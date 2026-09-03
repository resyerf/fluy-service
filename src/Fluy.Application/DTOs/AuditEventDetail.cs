namespace Fluy.Application.DTOs;

public record AuditEventDetail(
    Guid Id, Guid? UserId, string Action, string EntityType, Guid EntityId,
    string? PreviousState, string? NewState, string? Metadata, string? Reason, string? Comment, DateTimeOffset CreatedAt);
