using Fluy.Application.Organization.GetDepartmentsByBranch;
using Fluy.Domain.Tenancy;

namespace Fluy.Application.Common.Interfaces.Repositories;

public interface IDepartmentRepository
{
    void Add(Department department);
    Task<IReadOnlyCollection<DepartmentDetail>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken);
}
