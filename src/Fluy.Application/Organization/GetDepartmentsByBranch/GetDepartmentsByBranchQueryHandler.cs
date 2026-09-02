using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.GetDepartmentsByBranch;

public class GetDepartmentsByBranchQueryHandler(IDepartmentRepository departments)
    : IQueryHandler<GetDepartmentsByBranchQuery, IReadOnlyCollection<DepartmentDetail>>
{
    public Task<IReadOnlyCollection<DepartmentDetail>> Handle(GetDepartmentsByBranchQuery query, CancellationToken cancellationToken) =>
        departments.GetByBranchAsync(query.BranchId, cancellationToken);
}
