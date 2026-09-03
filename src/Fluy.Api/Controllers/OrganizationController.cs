using Fluy.Application.Common.Exceptions;
using Fluy.Application.Commands.Organization.CreateBranch;
using Fluy.Application.DTOs;
using Fluy.Application.Commands.Organization.CreateCompany;
using Fluy.Application.Commands.Organization.CreateDepartment;
using Fluy.Application.Queries.Organization.GetAllBranches;
using Fluy.Application.Queries.Organization.GetBranchesByCompany;
using Fluy.Application.Queries.Organization.GetCompanies;
using Fluy.Application.Queries.Organization.GetDepartmentsByBranch;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fluy.Api.Models.Requests;

namespace Fluy.Api.Controllers;

[ApiController]
[Route("api/v1/organization")]
[Authorize]
public class OrganizationController(ISender sender) : ControllerBase
{

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
