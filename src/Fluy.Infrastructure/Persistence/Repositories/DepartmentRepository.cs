using Fluy.Application.Interfaces.Repositories;
using Fluy.Application.DTOs;
using Fluy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Fluy.Infrastructure.Persistence.Context;

namespace Fluy.Infrastructure.Persistence.Repositories;

internal sealed class DepartmentRepository(ApplicationDbContext db) : IDepartmentRepository
{
    public void Add(Department department) => db.Departments.Add(department);

    public async Task<IReadOnlyCollection<DepartmentDetail>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken) =>
        await db.Departments.AsNoTracking()
            .Where(d => d.BranchId == branchId)
            .Select(d => new DepartmentDetail(d.Id, d.Name))
            .ToListAsync(cancellationToken);
}
