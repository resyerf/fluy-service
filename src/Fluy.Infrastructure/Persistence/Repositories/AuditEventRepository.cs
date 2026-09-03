using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Fluy.Infrastructure.Persistence.Context;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class AuditEventRepository(ApplicationDbContext db) : IAuditEventRepository
{
    public void Add(AuditEvent auditEvent) => db.AuditEvents.Add(auditEvent);

    public async Task<IReadOnlyCollection<AuditEventDetail>> GetByEntityAsync(
        string entityType, Guid entityId, CancellationToken cancellationToken) =>
        await db.AuditEvents.AsNoTracking().Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AuditEventDetail(
                a.Id, a.UserId, a.Action, a.EntityType, a.EntityId, a.PreviousState, a.NewState, a.Metadata, a.Reason, a.Comment, a.CreatedAt))
            .ToListAsync(cancellationToken);
}
