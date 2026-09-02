using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.GetBranchesByCompany;

public record GetBranchesByCompanyQuery(Guid CompanyId) : IQuery<IReadOnlyCollection<BranchDetail>>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}

public record BranchDetail(Guid Id, string Name);
