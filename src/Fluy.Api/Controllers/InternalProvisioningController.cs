using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Commands.Identity.BootstrapTenant;
using Fluy.Application.Commands.Identity.SeedDemoTenantData;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Mvc;
using Fluy.Api.Models.Requests;

namespace Fluy.Api.Controllers;

/// <summary>
/// Usado exclusivamente por fluy-admin-service durante el aprovisionamiento de un tenant nuevo
/// (CODE.md §9.5). Protegido por InternalApiKeyMiddleware, no por el esquema JWT de usuarios/tenants.
/// </summary>
[ApiController]
[Route("api/internal/provisioning")]
public class InternalProvisioningController(ISender sender, ICurrentTenantService currentTenant) : ControllerBase
{

    [HttpPost("tenants/{tenantId:guid}/bootstrap")]
    public async Task<ActionResult<BootstrapTenantResult>> Bootstrap(
        Guid tenantId, BootstrapRequest request, CancellationToken cancellationToken)
    {
        currentTenant.SetTenant(tenantId);

        var result = await sender.Send(
            new BootstrapTenantCommand(request.MasterEmail, request.MasterFullName), cancellationToken);

        return Ok(result);
    }

    /// <summary>Llamado por DemoTenantSeeder (fluy-admin-service) solo en Development, justo después de Bootstrap.</summary>
    [HttpPost("tenants/{tenantId:guid}/seed-demo-data")]
    public async Task<ActionResult<SeedDemoTenantDataResult>> SeedDemoData(Guid tenantId, CancellationToken cancellationToken)
    {
        currentTenant.SetTenant(tenantId);

        var result = await sender.Send(new SeedDemoTenantDataCommand(), cancellationToken);

        return Ok(result);
    }
}
