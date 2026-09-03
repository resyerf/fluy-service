namespace Fluy.Application.DTOs;

public record DocumentDetail(
    Guid Id, string FileName, string ContentType, long SizeBytes, Guid UploadedByUserId, int Version, DateTimeOffset CreatedAt);
