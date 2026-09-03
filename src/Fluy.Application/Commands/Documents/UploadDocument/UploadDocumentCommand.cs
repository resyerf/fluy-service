using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Documents.UploadDocument;

/// <summary>CLAUDE.md §21 (Documentos) — adjunto de una Request. Content se lee una sola vez por IDocumentStorage.SaveAsync.</summary>
public record UploadDocumentCommand(Guid RequestId, string FileName, string ContentType, long SizeBytes, Stream Content)
    : ICommand<UploadDocumentResult>, IRequiresPermission
{
    public string PermissionCode => "document.upload";
}
