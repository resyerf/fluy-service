using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Tenancy;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.CreateBranch;

public class CreateBranchCommandHandler(
    IBranchRepository branches, ICompanyRepository companies, IUnitOfWork unitOfWork, ICurrentTenantService currentTenant)
    : ICommandHandler<CreateBranchCommand, CreateBranchResult>
{
    public async Task<CreateBranchResult> Handle(CreateBranchCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var companyExists = await companies.ExistsAsync(command.CompanyId, cancellationToken);
        if (!companyExists)
        {
            throw new CompanyNotFoundException(command.CompanyId);
        }

        var branch = Branch.Create(tenantId, command.CompanyId, command.Name);
        branches.Add(branch);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateBranchResult(branch.Id);
    }
}
