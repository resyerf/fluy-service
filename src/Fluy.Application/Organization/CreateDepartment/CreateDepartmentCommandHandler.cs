using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Tenancy;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.CreateDepartment;

public class CreateDepartmentCommandHandler(
    IDepartmentRepository departments, IBranchRepository branches, IUnitOfWork unitOfWork, ICurrentTenantService currentTenant)
    : ICommandHandler<CreateDepartmentCommand, CreateDepartmentResult>
{
    public async Task<CreateDepartmentResult> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var branchExists = await branches.ExistsAsync(command.BranchId, cancellationToken);
        if (!branchExists)
        {
            throw new BranchNotFoundException(command.BranchId);
        }

        var department = Department.Create(tenantId, command.BranchId, command.Name);
        departments.Add(department);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateDepartmentResult(department.Id);
    }
}
