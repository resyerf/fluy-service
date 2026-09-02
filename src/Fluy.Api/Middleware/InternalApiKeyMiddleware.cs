namespace Fluy.Api.Middleware;

/// <summary>
/// Autenticación servicio-a-servicio (CODE.md §10-D14, elegida: API key compartida). Protege
/// exclusivamente las rutas api/internal/* — nunca alcanzables por un tenant ni por un usuario
/// final, y que además deberían bloquearse a nivel de red/gateway en producción (defensa en
/// profundidad, CODE.md §4.24/§9.6). Se ejecuta antes que cualquier otro middleware.
/// </summary>
public class InternalApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private const string ApiKeyHeader = "X-Service-Api-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api/internal"))
        {
            await next(context);
            return;
        }

        var expectedKey = configuration["Provisioning:ApiKey"];
        var providedKey = context.Request.Headers[ApiKeyHeader].ToString();

        if (string.IsNullOrEmpty(expectedKey) || !string.Equals(providedKey, expectedKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { status = 401, detail = "API key de servicio inválida o ausente." });
            return;
        }

        await next(context);
    }
}
