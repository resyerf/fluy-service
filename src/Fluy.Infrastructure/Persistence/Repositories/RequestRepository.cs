using Fluy.Application.Interfaces.Repositories;
using Fluy.Application.DTOs;
using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Fluy.Infrastructure.Persistence.Context;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class RequestRepository(ApplicationDbContext db) : IRequestRepository
{
    public Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Requests.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public void Add(Request request) => db.Requests.Add(request);

    public void AddFields(IEnumerable<RequestField> fields) => db.RequestFields.AddRange(fields);

    public async Task<IReadOnlyCollection<RequestFieldDetail>> GetFieldsAsync(Guid requestId, CancellationToken cancellationToken) =>
        await db.RequestFields.AsNoTracking()
            .Where(f => f.RequestId == requestId)
            .Select(f => new RequestFieldDetail(f.Key, f.Value))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<RequestSummary>> GetMineAsync(
        Guid requesterId, Guid? branchId, CancellationToken cancellationToken) =>
        await db.Requests.AsNoTracking()
            .Where(r => r.RequesterId == requesterId)
            .Where(r => branchId == null || r.BranchId == branchId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RequestSummary(r.Id, r.Title, r.Amount, r.Status.ToString(), r.SubmittedAt))
            .ToListAsync(cancellationToken);
}
