using Fluy.Application.Common.Exceptions;
using FluentValidation;

namespace Fluy.Api.Middlewares;

/// <summary>
/// Traduce las excepciones que puede lanzar el Dispatcher compartido (Fluy.SharedKernel) antes de
/// llegar al handler — ValidationException (FluentValidation) y NotAuthorizedException (D19) — a
/// respuestas HTTP con formato ProblemDetails (CODE.md §4.23), en vez de dejarlas caer como 500.
/// Excepciones específicas de un solo endpoint (AuthenticationFailedException, etc.) se siguen
/// manejando en el controller correspondiente, no acá.
/// </summary>
public class ApplicationExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                status = 400,
                title = "Uno o más campos no son válidos.",
                errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            });
        }
        catch (NotAuthorizedException ex)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { status = 403, detail = ex.Message });
        }
        catch (RequiredRoleNotHeldException ex)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { status = 403, detail = ex.Message });
        }
    }
}
