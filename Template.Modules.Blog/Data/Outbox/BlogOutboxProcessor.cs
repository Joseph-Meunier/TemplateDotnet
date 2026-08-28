using Microsoft.EntityFrameworkCore;
using Template.Shared.Events;

namespace Template.Modules.Blog.Data.Outbox;

internal sealed class BlogOutboxProcessor(
    BlogDbContext dbContext,
    IIntegrationEventPublisher publisher)
    : IOutboxProcessor
{
    public async Task ProcessAsync(
        CancellationToken cancellationToken)
    {
        var messages = await dbContext.OutboxMessages
            .Where(x => x.ProcessedAt == null)
            .OrderBy(x => x.OccurredAt)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            await publisher.PublishAsync(
                message.Id,
                message.Type,
                message.Payload,
                cancellationToken);

            message.MarkAsProcessed(
                DateTimeOffset.UtcNow);
        }

        if (messages.Count > 0)
        {
            await dbContext.SaveChangesAsync(
                cancellationToken);
        }
    }
}