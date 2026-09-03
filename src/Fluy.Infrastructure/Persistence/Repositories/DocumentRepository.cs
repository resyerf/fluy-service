using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Fluy.Infrastructure.Persistence.Context;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class DocumentRepository(ApplicationDbContext db) : IDocumentRepository
{
    public void Add(Document document) => db.Documents.Add(document);

    public Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Documents.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<DocumentDetail>> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken) =>
        await db.Documents.AsNoTracking().Where(d => d.RequestId == requestId).OrderByDescending(d => d.CreatedAt)
            .Select(d => new DocumentDetail(d.Id, d.FileName, d.ContentType, d.SizeBytes, d.UploadedByUserId, d.Version, d.CreatedAt))
            .ToListAsync(cancellationToken);
}
