using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Organization.GetDepartmentsByBranch;

public record GetDepartmentsByBranchQuery(Guid BranchId) : IQuery<IReadOnlyCollection<DepartmentDetail>>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}
