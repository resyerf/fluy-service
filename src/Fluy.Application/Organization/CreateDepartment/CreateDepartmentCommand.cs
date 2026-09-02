using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.CreateDepartment;

public record CreateDepartmentCommand(Guid BranchId, string Name) : ICommand<CreateDepartmentResult>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}

public record CreateDepartmentResult(Guid DepartmentId);
