using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Queries.Rules.GetApprovalRule;
using Fluy.Application.Commands.Rules.SetApprovalRule;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fluy.Api.Models.Requests;

namespace Fluy.Api.Controllers;

/// <summary>Rules Engine mínimo (CLAUDE.md §16, CODE.md §9.19) — hoy solo la ApprovalRule de escalamiento por monto.</summary>
[ApiController]
[Route("api/v1/rules")]
[Authorize]
public class RulesController(ISender sender) : ControllerBase
{

    [HttpGet("approval")]
    public async Task<ActionResult<ApprovalRuleDetail?>> GetApprovalRule(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetApprovalRuleQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("approval")]
    public async Task<ActionResult<SetApprovalRuleResult>> SetApprovalRule(SetApprovalRuleBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new SetApprovalRuleCommand(body.MinAmount, body.SecondApproverRoleId), cancellationToken);
            return Ok(result);
        }
        catch (RoleNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }
}
