using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Organization.CreateDepartment;

public record CreateDepartmentCommand(Guid BranchId, string Name) : ICommand<CreateDepartmentResult>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}
