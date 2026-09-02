using Fluy.Application.Approvals.ApproveRequest;
using Fluy.Application.Approvals.GetPendingApprovals;
using Fluy.Application.Approvals.RejectRequest;
using Fluy.Application.Approvals.RequestCorrection;
using Fluy.Application.Common.Exceptions;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fluy.Api.Controllers;

[ApiController]
[Route("api/v1/approvals")]
[Authorize]
public class ApprovalsController(ISender sender) : ControllerBase
{
    public record DecisionBody(string? Comment);

    [HttpGet("pending")]
    public async Task<ActionResult<IReadOnlyCollection<PendingApprovalSummary>>> GetPending(
        [FromQuery] Guid? branchId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPendingApprovalsQuery(branchId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{requestId:guid}/approve")]
    public async Task<ActionResult<ApproveRequestResult>> Approve(
        Guid requestId, DecisionBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new ApproveRequestCommand(requestId, body.Comment), cancellationToken);
            return Ok(result);
        }
        catch (ApprovalNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (RequestNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }

    [HttpPost("{requestId:guid}/reject")]
    public async Task<ActionResult<RejectRequestResult>> Reject(
        Guid requestId, DecisionBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new RejectRequestCommand(requestId, body.Comment ?? string.Empty), cancellationToken);
            return Ok(result);
        }
        catch (ApprovalNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (RequestNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }

    [HttpPost("{requestId:guid}/request-correction")]
    public async Task<ActionResult<RequestCorrectionResult>> RequestCorrection(
        Guid requestId, DecisionBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new RequestCorrectionCommand(requestId, body.Comment ?? string.Empty), cancellationToken);
            return Ok(result);
        }
        catch (ApprovalNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (RequestNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }
}
