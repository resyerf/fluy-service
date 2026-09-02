using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.CreateBranch;

public record CreateBranchCommand(Guid CompanyId, string Name) : ICommand<CreateBranchResult>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}

public record CreateBranchResult(Guid BranchId);
