using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Organization.CreateCompany;

public record CreateCompanyCommand(string Name, string? LegalIdentifier) : ICommand<CreateCompanyResult>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}
