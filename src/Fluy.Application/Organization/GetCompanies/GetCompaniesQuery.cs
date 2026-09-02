using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.GetCompanies;

public record GetCompaniesQuery : IQuery<IReadOnlyCollection<CompanyDetail>>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}

public record CompanyDetail(Guid Id, string Name, string? LegalIdentifier);
