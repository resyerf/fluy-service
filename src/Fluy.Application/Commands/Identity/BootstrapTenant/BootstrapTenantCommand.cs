using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Identity.BootstrapTenant;

/// <summary>
/// Invocado exclusivamente por InternalProvisioningController (fluy-admin-service → fluy-service,
/// CODE.md §9.5-9.6). El TenantId no viaja en el command: lo fija el controller en
/// ICurrentTenantService antes de despachar, igual que hace ApplicationDbContextInitializer al
/// sembrar el tenant demo.
/// </summary>
public record BootstrapTenantCommand(string MasterEmail, string MasterFullName) : ICommand<BootstrapTenantResult>;
