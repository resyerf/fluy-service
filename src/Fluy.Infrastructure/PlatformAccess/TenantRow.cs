namespace Fluy.Infrastructure.PlatformAccess;

/// <summary>
/// Proyección de solo lectura de platform.Tenants (tabla propiedad de fluy-admin-service).
/// No es un agregado de dominio — no tiene comportamiento, es un espejo mínimo de columnas.
/// </summary>
public class TenantRow
{
    public Guid Id { get; set; }
    public string Subdomain { get; set; } = null!;
    public string Status { get; set; } = null!;
}
