using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IAuditEventRepository
{
    void Add(AuditEvent auditEvent);
    Task<IReadOnlyCollection<AuditEventDetail>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken);
}
