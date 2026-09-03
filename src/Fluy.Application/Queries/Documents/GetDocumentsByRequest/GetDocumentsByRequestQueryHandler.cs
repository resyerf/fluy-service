using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Queries.Documents.GetDocumentsByRequest;

public class GetDocumentsByRequestQueryHandler(IDocumentRepository documents)
    : IQueryHandler<GetDocumentsByRequestQuery, IReadOnlyCollection<DocumentDetail>>
{
    public Task<IReadOnlyCollection<DocumentDetail>> Handle(GetDocumentsByRequestQuery query, CancellationToken cancellationToken) =>
        documents.GetByRequestIdAsync(query.RequestId, cancellationToken);
}
