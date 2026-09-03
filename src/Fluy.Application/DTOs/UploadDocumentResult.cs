namespace Fluy.Application.DTOs;

public record UploadDocumentResult(Guid DocumentId, string FileName, long SizeBytes);
