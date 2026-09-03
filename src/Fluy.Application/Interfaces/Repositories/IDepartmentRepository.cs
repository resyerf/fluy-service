using Fluy.Application.DTOs;
using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Repositories;

public interface IDepartmentRepository
{
    void Add(Department department);
    Task<IReadOnlyCollection<DepartmentDetail>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken);
}
