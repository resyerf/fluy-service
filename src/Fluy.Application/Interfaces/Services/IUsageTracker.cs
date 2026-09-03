namespace Fluy.Application.Interfaces.Services;

/// <summary>
/// Puerto de consumo (CODE.md §4.16). Incrementa un contador de uso del tenant actual en
/// platform.UsageRecord — la única escritura cross-schema permitida (CODE.md §9.4, excepción #3),
/// acotada a un UPDATE/upsert de contador, nunca a crear/borrar el catálogo de Feature.
/// </summary>
public interface IUsageTracker
{
    Task IncrementAsync(Guid tenantId, string metricCode, int amount, CancellationToken cancellationToken);
}
