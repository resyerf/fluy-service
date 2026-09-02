using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.CreateCompany;

public record CreateCompanyCommand(string Name, string? LegalIdentifier) : ICommand<CreateCompanyResult>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}

public record CreateCompanyResult(Guid CompanyId);
