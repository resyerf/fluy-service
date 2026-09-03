using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Organization.GetCompanies;

public record GetCompaniesQuery : IQuery<IReadOnlyCollection<CompanyDetail>>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}
