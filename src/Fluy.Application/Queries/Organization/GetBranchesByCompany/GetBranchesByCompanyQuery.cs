using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Organization.GetBranchesByCompany;

public record GetBranchesByCompanyQuery(Guid CompanyId) : IQuery<IReadOnlyCollection<BranchDetail>>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}
