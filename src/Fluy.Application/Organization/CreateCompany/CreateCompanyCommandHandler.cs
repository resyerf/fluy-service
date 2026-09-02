using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Tenancy;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.CreateCompany;

public class CreateCompanyCommandHandler(
    ICompanyRepository companies, IUnitOfWork unitOfWork, ICurrentTenantService currentTenant)
    : ICommandHandler<CreateCompanyCommand, CreateCompanyResult>
{
    public async Task<CreateCompanyResult> Handle(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var company = Company.Create(tenantId, command.Name, command.LegalIdentifier);
        companies.Add(company);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateCompanyResult(company.Id);
    }
}
