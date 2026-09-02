using Fluy.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fluy.Api.Controllers;

/// <summary>
/// Endpoint de diagnóstico para validar de punta a punta la lectura de platform.Subscriptions/
/// platform.PlanFeatures (CODE.md §9.4). El consumo real (bloquear una acción si falta un
/// feature) llegará con EntitlementBehavior, todavía no implementado — no hay un comando de
/// negocio real al que engancharlo hasta que exista Requests/Workflows.
/// </summary>
[ApiController]
[Route("api/v1/entitlements")]
[Authorize]
public class EntitlementsController(IEntitlementReader entitlementReader, ICurrentTenantService currentTenant) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;
        var entitlements = await entitlementReader.GetEntitlementsAsync(tenantId, cancellationToken);

        return Ok(new
        {
            tenantId,
            entitlements = entitlements.ToDictionary(e => e.FeatureCode, e => e.Value)
        });
    }
}
