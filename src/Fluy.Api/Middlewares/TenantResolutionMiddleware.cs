using Fluy.Application.Interfaces.Services;

namespace Fluy.Api.Middlewares;

/// <summary>
/// Única puerta de entrada del tenant al resto del sistema (CODE.md §4.6). Resuelve el subdominio
/// (acme.fluy.com → "acme") o, en desarrollo local sin subdominios reales, el header "X-Tenant".
/// Debe registrarse antes de UseAuthentication. Las rutas api/internal/* (llamadas exclusivamente
/// por fluy-admin-service, ver InternalApiKeyMiddleware) no tienen subdominio que resolver: el
/// tenant llega explícito en la ruta y lo fija el propio controller.
/// </summary>
public class TenantResolutionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantDirectory tenantDirectory, ICurrentTenantService currentTenant)
    {
        if (context.Request.Path.StartsWithSegments("/api/internal"))
        {
            await next(context);
            return;
        }

        var subdomain = ResolveSubdomain(context);

        if (subdomain is null)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "No se pudo determinar el tenant de la request.");
            return;
        }

        var tenant = await tenantDirectory.FindBySubdomainAsync(subdomain, context.RequestAborted);

        if (tenant is null || !tenant.IsActive)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, $"Tenant '{subdomain}' no existe o no está activo.");
            return;
        }

        currentTenant.SetTenant(tenant.Id);
        await next(context);
    }

    private static string? ResolveSubdomain(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Tenant", out var headerValue) && !string.IsNullOrWhiteSpace(headerValue))
        {
            return headerValue.ToString().Trim().ToLowerInvariant();
        }

        var host = context.Request.Host.Host;
        var labels = host.Split('.', StringSplitOptions.RemoveEmptyEntries);

        return labels.Length >= 3 ? labels[0].ToLowerInvariant() : null;
    }

    private static Task WriteProblemAsync(HttpContext context, int statusCode, string detail)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new { status = statusCode, detail });
    }
}
