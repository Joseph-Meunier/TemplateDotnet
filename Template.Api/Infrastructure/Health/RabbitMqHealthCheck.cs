using Microsoft.Extensions.Diagnostics.HealthChecks;
using Template.Api.Infrastructure.Messaging;

namespace Template.Api.Infrastructure.Health;

public sealed class RabbitMqHealthCheck(
    RabbitMqConnection connection)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rabbitConnection =
                await connection.GetConnectionAsync(
                    cancellationToken);

            return rabbitConnection.IsOpen
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy(
                    "RabbitMQ connection is closed.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                "RabbitMQ is unavailable.",
                exception);
        }
    }
}