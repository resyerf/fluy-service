using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.GetDepartmentsByBranch;

public record GetDepartmentsByBranchQuery(Guid BranchId) : IQuery<IReadOnlyCollection<DepartmentDetail>>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}

public record DepartmentDetail(Guid Id, string Name);
