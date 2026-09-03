using Fluy.Application.Commands.Documents.UploadDocument;
using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Queries.Documents.GetDocumentsByRequest;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fluy.Api.Controllers;

/// <summary>Adjuntos de una Request (CLAUDE.md §21).</summary>
[ApiController]
[Route("api/v1/requests/{requestId:guid}/documents")]
[Authorize]
public class DocumentsController(ISender sender, IDocumentRepository documents, IDocumentStorage storage) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<DocumentDetail>>> GetByRequest(Guid requestId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDocumentsByRequestQuery(requestId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<ActionResult<UploadDocumentResult>> Upload(Guid requestId, IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = file.OpenReadStream();
            var result = await sender.Send(
                new UploadDocumentCommand(requestId, file.FileName, file.ContentType, file.Length, stream), cancellationToken);
            return Ok(result);
        }
        catch (RequestNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }

    [HttpGet("{documentId:guid}/download")]
    public async Task<IActionResult> Download(Guid requestId, Guid documentId, CancellationToken cancellationToken)
    {
        var document = await documents.GetByIdAsync(documentId, cancellationToken);
        if (document is null || document.RequestId != requestId)
        {
            return NotFound();
        }

        var stream = await storage.OpenAsync(document.StorageKey, cancellationToken);
        return File(stream, document.ContentType, document.FileName);
    }
}
