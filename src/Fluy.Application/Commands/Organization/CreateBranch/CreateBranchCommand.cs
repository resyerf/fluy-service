using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Organization.CreateBranch;

public record CreateBranchCommand(Guid CompanyId, string Name) : ICommand<CreateBranchResult>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}
