using Template.Shared.Events;

namespace Template.Api.Infrastructure.Messaging;

public sealed class OutboxWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxesAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "An error occurred while processing outboxes.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken);
        }
    }

    private async Task ProcessOutboxesAsync(
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var processors = scope.ServiceProvider
            .GetServices<IOutboxProcessor>();

        foreach (var processor in processors)
        {
            await processor.ProcessAsync(
                cancellationToken);
        }
    }
}