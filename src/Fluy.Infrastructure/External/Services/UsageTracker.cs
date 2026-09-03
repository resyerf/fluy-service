using Fluy.Application.Interfaces.Services;
using Fluy.SharedKernel;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Fluy.Infrastructure.External.Services;

/// <summary>
/// Adapter de IUsageTracker (CODE.md §4.16, §9.4 excepción #3): usa la misma cadena de conexión
/// ("ApplicationDb") que PlatformReadDbContext para llegar al schema "platform" de la misma
/// instancia de Postgres, pero vía SQL crudo en vez de EF Core — un upsert de contador acotado,
/// nunca una entidad completa. platform.UsageRecords lo migra y gestiona FluyAdmin.Infrastructure.
/// </summary>
public class UsageTracker(IConfiguration configuration, IDateTime dateTime) : IUsageTracker
{
    private const string UpsertSql = """
        INSERT INTO platform."UsageRecords" ("Id", "TenantId", "MetricCode", "Period", "Count")
        VALUES (@id, @tenantId, @metricCode, @period, @amount)
        ON CONFLICT ("TenantId", "MetricCode", "Period")
        DO UPDATE SET "Count" = platform."UsageRecords"."Count" + @amount
        """;

    public async Task IncrementAsync(Guid tenantId, string metricCode, int amount, CancellationToken cancellationToken)
    {
        var period = dateTime.UtcNow.ToString("yyyy-MM");

        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("ApplicationDb"));
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(UpsertSql, connection);
        command.Parameters.AddWithValue("id", Guid.NewGuid());
        command.Parameters.AddWithValue("tenantId", tenantId);
        command.Parameters.AddWithValue("metricCode", metricCode);
        command.Parameters.AddWithValue("period", period);
        command.Parameters.AddWithValue("amount", amount);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
