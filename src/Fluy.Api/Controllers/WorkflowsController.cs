using Fluy.Application.Common.Exceptions;
using Fluy.Application.Commands.Workflows.AddWorkflowStep;
using Fluy.Application.DTOs;
using Fluy.Application.Commands.Workflows.AddWorkflowTransition;
using Fluy.Application.Commands.Workflows.ArchiveWorkflowDefinition;
using Fluy.Application.Commands.Workflows.CreateWorkflowDefinition;
using Fluy.Application.Queries.Workflows.GetWorkflowDefinitions;
using Fluy.Application.Queries.Workflows.GetWorkflowVersionDetail;
using Fluy.Application.Commands.Workflows.PublishWorkflowVersion;
using Fluy.Application.Commands.Workflows.SetInitialStep;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fluy.Api.Models.Requests;

namespace Fluy.Api.Controllers;

/// <summary>Workflow Engine genérico (CLAUDE.md §14-16, CODE.md §9.20) — reemplaza el escalamiento hardcodeado de <c>ApprovalRule</c>.</summary>
[ApiController]
[Route("api/v1/workflows")]
[Authorize]
public class WorkflowsController(ISender sender) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<WorkflowDefinitionSummary>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetWorkflowDefinitionsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateWorkflowDefinitionResult>> Create(
        CreateWorkflowDefinitionBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateWorkflowDefinitionCommand(body.Name, body.Description), cancellationToken);
        return Ok(result);
    }

    [HttpGet("versions/{versionId:guid}")]
    public async Task<ActionResult<WorkflowVersionDetail>> GetVersion(Guid versionId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new GetWorkflowVersionDetailQuery(versionId), cancellationToken);
            return Ok(result);
        }
        catch (WorkflowVersionNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }

    [HttpPost("versions/{versionId:guid}/steps")]
    public async Task<ActionResult<AddWorkflowStepResult>> AddStep(
        Guid versionId, AddWorkflowStepBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new AddWorkflowStepCommand(versionId, body.Name, body.ApproverRoleId), cancellationToken);
            return Ok(result);
        }
        catch (WorkflowVersionNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (RoleNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidWorkflowStateException ex)
        {
            return Conflict(new { detail = ex.Message });
        }
    }

    [HttpPost("versions/{versionId:guid}/transitions")]
    public async Task<ActionResult<AddWorkflowTransitionResult>> AddTransition(
        Guid versionId, AddWorkflowTransitionBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new AddWorkflowTransitionCommand(
                    versionId, body.FromStepId, body.ToStepId,
                    body.ConditionField, body.ConditionOperator, body.ConditionValue, body.Order),
                cancellationToken);
            return Ok(result);
        }
        catch (WorkflowVersionNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (WorkflowStepNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidWorkflowStateException ex)
        {
            return Conflict(new { detail = ex.Message });
        }
    }

    [HttpPut("versions/{versionId:guid}/initial-step")]
    public async Task<ActionResult<SetInitialStepResult>> SetInitialStep(
        Guid versionId, SetInitialStepBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new SetInitialStepCommand(versionId, body.StepId), cancellationToken);
            return Ok(result);
        }
        catch (WorkflowVersionNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (WorkflowStepNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidWorkflowStateException ex)
        {
            return Conflict(new { detail = ex.Message });
        }
    }

    [HttpPost("versions/{versionId:guid}/publish")]
    public async Task<ActionResult<PublishWorkflowVersionResult>> Publish(Guid versionId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new PublishWorkflowVersionCommand(versionId), cancellationToken);
            return Ok(result);
        }
        catch (WorkflowVersionNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (WorkflowDefinitionNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidWorkflowStateException ex)
        {
            return Conflict(new { detail = ex.Message });
        }
    }

    [HttpPost("{definitionId:guid}/archive")]
    public async Task<ActionResult<ArchiveWorkflowDefinitionResult>> Archive(Guid definitionId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new ArchiveWorkflowDefinitionCommand(definitionId), cancellationToken);
            return Ok(result);
        }
        catch (WorkflowDefinitionNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }
}
