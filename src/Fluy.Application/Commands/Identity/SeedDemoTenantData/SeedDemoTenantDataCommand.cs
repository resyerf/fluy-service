using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Identity.SeedDemoTenantData;

/// <summary>
/// Invocado exclusivamente por InternalProvisioningController (fluy-admin-service → fluy-service),
/// después de BootstrapTenant, al sembrar el tenant demo (subdominio "demo") en Development. El
/// TenantId no viaja en el command: lo fija el controller en ICurrentTenantService antes de
/// despachar, igual que BootstrapTenantCommand. Todo el contenido demo es constante (ver el
/// handler) — no hay nada que parametrizar.
/// </summary>
public record SeedDemoTenantDataCommand : ICommand<SeedDemoTenantDataResult>;
