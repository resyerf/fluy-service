using Fluy.Application.Common.Exceptions;
using Fluy.Application.Commands.Requests.CreateRequest;
using Fluy.Application.DTOs;
using Fluy.Application.Queries.Requests.GetMyRequests;
using Fluy.Application.Queries.Requests.GetRequestById;
using Fluy.Application.Commands.Requests.SubmitRequest;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fluy.Api.Models.Requests;

namespace Fluy.Api.Controllers;

[ApiController]
[Route("api/v1/requests")]
[Authorize]
public class RequestsController(ISender sender) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<CreateRequestResult>> Create(CreateRequestBody body, CancellationToken cancellationToken)
    {
        try
        {
            var fields = body.Fields?.Select(f => new CreateRequestFieldInput(f.Key, f.Value)).ToList();
            var result = await sender.Send(
                new CreateRequestCommand(body.Title, body.Description, body.Amount, fields, body.BranchId), cancellationToken);
            return Ok(result);
        }
        catch (BranchNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<SubmitRequestResult>> Submit(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new SubmitRequestCommand(id), cancellationToken);
            return Ok(result);
        }
        catch (RequestNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidRequestStateException ex)
        {
            return Conflict(new { detail = ex.Message });
        }
    }

    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyCollection<RequestSummary>>> GetMine(
        [FromQuery] Guid? branchId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyRequestsQuery(branchId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestDetail>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new GetRequestByIdQuery(id), cancellationToken);
            return Ok(result);
        }
        catch (RequestNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }
}
