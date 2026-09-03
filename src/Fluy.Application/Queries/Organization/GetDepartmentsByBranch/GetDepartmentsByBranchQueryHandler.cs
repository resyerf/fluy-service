using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Organization.GetDepartmentsByBranch;

public class GetDepartmentsByBranchQueryHandler(IDepartmentRepository departments)
    : IQueryHandler<GetDepartmentsByBranchQuery, IReadOnlyCollection<DepartmentDetail>>
{
    public Task<IReadOnlyCollection<DepartmentDetail>> Handle(GetDepartmentsByBranchQuery query, CancellationToken cancellationToken) =>
        departments.GetByBranchAsync(query.BranchId, cancellationToken);
}
