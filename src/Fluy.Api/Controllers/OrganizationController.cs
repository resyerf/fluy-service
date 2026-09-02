using Fluy.Application.Common.Exceptions;
using Fluy.Application.Organization.CreateBranch;
using Fluy.Application.Organization.CreateCompany;
using Fluy.Application.Organization.CreateDepartment;
using Fluy.Application.Organization.GetAllBranches;
using Fluy.Application.Organization.GetBranchesByCompany;
using Fluy.Application.Organization.GetCompanies;
using Fluy.Application.Organization.GetDepartmentsByBranch;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fluy.Api.Controllers;

[ApiController]
[Route("api/v1/organization")]
[Authorize]
public class OrganizationController(ISender sender) : ControllerBase
{
    public record CreateCompanyBody(string Name, string? LegalIdentifier);
    public record CreateBranchBody(string Name);
    public record CreateDepartmentBody(string Name);

    [HttpPost("companies")]
    public async Task<ActionResult<CreateCompanyResult>> CreateCompany(CreateCompanyBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateCompanyCommand(body.Name, body.LegalIdentifier), cancellationToken);
        return Ok(result);
    }

    [HttpGet("companies")]
    public async Task<ActionResult<IReadOnlyCollection<CompanyDetail>>> GetCompanies(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCompaniesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("branches")]
    public async Task<ActionResult<IReadOnlyCollection<AllBranchDetail>>> GetAllBranches(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllBranchesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("companies/{companyId:guid}/branches")]
    public async Task<ActionResult<CreateBranchResult>> CreateBranch(
        Guid companyId, CreateBranchBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new CreateBranchCommand(companyId, body.Name), cancellationToken);
            return Ok(result);
        }
        catch (CompanyNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }

    [HttpGet("companies/{companyId:guid}/branches")]
    public async Task<ActionResult<IReadOnlyCollection<BranchDetail>>> GetBranches(Guid companyId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBranchesByCompanyQuery(companyId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("branches/{branchId:guid}/departments")]
    public async Task<ActionResult<CreateDepartmentResult>> CreateDepartment(
        Guid branchId, CreateDepartmentBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new CreateDepartmentCommand(branchId, body.Name), cancellationToken);
            return Ok(result);
        }
        catch (BranchNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }

    [HttpGet("branches/{branchId:guid}/departments")]
    public async Task<ActionResult<IReadOnlyCollection<DepartmentDetail>>> GetDepartments(Guid branchId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDepartmentsByBranchQuery(branchId), cancellationToken);
        return Ok(result);
    }
}
