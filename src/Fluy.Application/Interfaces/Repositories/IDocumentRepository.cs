using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IDocumentRepository
{
    void Add(Document document);
    Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DocumentDetail>> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken);
}
