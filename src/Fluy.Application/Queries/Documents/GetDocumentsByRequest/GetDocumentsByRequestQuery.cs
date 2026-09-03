using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Documents.GetDocumentsByRequest;

public record GetDocumentsByRequestQuery(Guid RequestId) : IQuery<IReadOnlyCollection<DocumentDetail>>, IRequiresPermission
{
    public string PermissionCode => "document.view";
}
