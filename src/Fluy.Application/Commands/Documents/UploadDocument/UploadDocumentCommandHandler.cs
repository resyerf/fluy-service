using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Documents.UploadDocument;

public class UploadDocumentCommandHandler(
    IRequestRepository requests, IDocumentRepository documents, IDocumentStorage storage,
    IUnitOfWork unitOfWork, ICurrentTenantService currentTenant, ICurrentUserService currentUser)
    : ICommandHandler<UploadDocumentCommand, UploadDocumentResult>
{
    public async Task<UploadDocumentResult> Handle(UploadDocumentCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;
        var userId = currentUser.UserId!.Value;

        var request = await requests.GetByIdAsync(command.RequestId, cancellationToken)
            ?? throw new RequestNotFoundException(command.RequestId);

        var storageKey = await storage.SaveAsync(tenantId, command.FileName, command.Content, cancellationToken);

        var document = Document.Create(tenantId, request.Id, command.FileName, command.ContentType, command.SizeBytes, storageKey, userId);
        documents.Add(document);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UploadDocumentResult(document.Id, document.FileName, document.SizeBytes);
    }
}
