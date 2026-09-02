using Fluy.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fluy.Infrastructure.Persistence;

/// <summary>
/// Aplica migraciones pendientes y siembra el catálogo global de permisos. Ya NO siembra ningún
/// tenant/usuario demo: la creación de tenants es responsabilidad exclusiva de fluy-admin-service
/// (CODE.md §9.2, §9.5) — probar el login end-to-end de fluy-service requiere aprovisionar un
/// tenant real vía ese flujo (o insertarlo manualmente en platform.Tenants para pruebas locales).
/// </summary>
public class ApplicationDbContextInitializer(
    ApplicationDbContext context,
    ILogger<ApplicationDbContextInitializer> logger)
{
    private static readonly (string Code, string Description)[] PermissionCatalog =
    [
        ("request.create", "Crear solicitudes"),
        ("request.view", "Consultar solicitudes"),
        ("request.approve", "Aprobar solicitudes"),
        ("request.reject", "Rechazar solicitudes"),
        ("organization.manage", "Administrar empresas, sedes y departamentos"),
        ("workflow.create", "Crear workflows"),
        ("workflow.edit", "Editar workflows"),
        ("workflow.publish", "Publicar workflows"),
        ("rules.manage", "Administrar reglas"),
        ("users.manage", "Administrar usuarios"),
        ("roles.manage", "Administrar roles y permisos"),
        ("audit.view", "Consultar auditoría"),
        ("billing.view", "Consultar facturación"),
        ("subscription.manage", "Administrar la suscripción del tenant")
    ];

    public async Task InitialiseAsync(CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existingCodes = await context.Permissions
            .IgnoreQueryFilters()
            .Select(p => p.Code)
            .ToListAsync(cancellationToken);

        var missing = PermissionCatalog.Where(p => !existingCodes.Contains(p.Code)).ToList();
        if (missing.Count == 0)
        {
            return;
        }

        context.Permissions.AddRange(missing.Select(p => Permission.Create(p.Code, p.Description)));
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Sembrados {Count} permisos nuevos en el catálogo global.", missing.Count);
    }
}
