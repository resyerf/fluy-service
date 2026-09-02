using Fluy.Application.Common.Interfaces;
using Fluy.Application.Identity.BootstrapTenant;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Mvc;

namespace Fluy.Api.Controllers;

/// <summary>
/// Usado exclusivamente por fluy-admin-service durante el aprovisionamiento de un tenant nuevo
/// (CODE.md §9.5). Protegido por InternalApiKeyMiddleware, no por el esquema JWT de usuarios/tenants.
/// </summary>
[ApiController]
[Route("api/internal/provisioning")]
public class InternalProvisioningController(ISender sender, ICurrentTenantService currentTenant) : ControllerBase
{
    public record BootstrapRequest(string MasterEmail, string MasterFullName);

    [HttpPost("tenants/{tenantId:guid}/bootstrap")]
    public async Task<ActionResult<BootstrapTenantResult>> Bootstrap(
        Guid tenantId, BootstrapRequest request, CancellationToken cancellationToken)
    {
        currentTenant.SetTenant(tenantId);

        var result = await sender.Send(
            new BootstrapTenantCommand(request.MasterEmail, request.MasterFullName), cancellationToken);

        return Ok(result);
    }
}
