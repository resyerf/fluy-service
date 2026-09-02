using Fluy.Application.Common.Exceptions;
using Fluy.Application.Identity.AssignRole;
using Fluy.Application.Identity.CreateRole;
using Fluy.Application.Identity.CreateUser;
using Fluy.Application.Identity.GetMyBranches;
using Fluy.Application.Identity.GetPermissionCatalog;
using Fluy.Application.Identity.GetRoles;
using Fluy.Application.Identity.GetUsers;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fluy.Api.Controllers;

/// <summary>
/// Administración de usuarios y roles del tenant (CLAUDE.md §32 "Organización": crear usuario,
/// crear rol, asignar permisos). Se llama "IdentityAdmin" y no "Identity" para no chocar con el
/// namespace Fluy.Application.Identity que ya existe para Login/BootstrapTenant/SetPassword.
/// </summary>
[ApiController]
[Route("api/v1/identity")]
[Authorize]
public class IdentityAdminController(ISender sender) : ControllerBase
{
    public record CreateUserBody(string Email, string FullName);
    public record CreateRoleBody(string Name, IReadOnlyCollection<string> PermissionCodes);
    public record AssignRoleBody(Guid UserId, Guid RoleId, Guid? BranchId, Guid? DepartmentId);

    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyCollection<UserDetail>>> GetUsers(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUsersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("users")]
    public async Task<ActionResult<CreateUserResult>> CreateUser(CreateUserBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new CreateUserCommand(body.Email, body.FullName), cancellationToken);
            return Ok(result);
        }
        catch (EmailAlreadyRegisteredException ex)
        {
            return Conflict(new { detail = ex.Message });
        }
    }

    [HttpGet("roles")]
    public async Task<ActionResult<IReadOnlyCollection<RoleDetail>>> GetRoles(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetRolesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("roles")]
    public async Task<ActionResult<CreateRoleResult>> CreateRole(CreateRoleBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new CreateRoleCommand(body.Name, body.PermissionCodes), cancellationToken);
            return Ok(result);
        }
        catch (UnknownPermissionCodesException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
    }

    [HttpGet("my-branches")]
    public async Task<ActionResult<IReadOnlyCollection<MyBranchSummary>>> GetMyBranches(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyBranchesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("permissions")]
    public async Task<ActionResult<IReadOnlyCollection<PermissionDetail>>> GetPermissionCatalog(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPermissionCatalogQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("user-roles")]
    public async Task<ActionResult<AssignRoleResult>> AssignRole(AssignRoleBody body, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new AssignRoleCommand(body.UserId, body.RoleId, body.BranchId, body.DepartmentId), cancellationToken);
            return Ok(result);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (RoleNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }
}
