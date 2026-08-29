using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Template.Modules.Blog.Contracts.Events;
using Template.Modules.Blog.Data;
using Template.Modules.Blog.Data.Inbox;
using Template.Shared.Events;

namespace Template.Modules.Blog.Messaging;

internal sealed class PostPublishedIntegrationEventHandler(
    BlogDbContext dbContext,
    ILogger<PostPublishedIntegrationEventHandler> logger)
    : IIntegrationEventHandler
{
    public string EventType =>
        typeof(PostPublishedIntegrationEvent).FullName!;

    public async Task HandleAsync(
        Guid messageId,
        string payload,
        CancellationToken cancellationToken)
    {
        var alreadyProcessed = await dbContext.InboxMessages
            .AsNoTracking()
            .AnyAsync(
                x => x.MessageId == messageId,
                cancellationToken);

        if (alreadyProcessed)
        {
            logger.LogInformation(
                "Integration event {MessageId} already processed.",
                messageId);

            return;
        }

        var integrationEvent =
            JsonSerializer.Deserialize<PostPublishedIntegrationEvent>(
                payload);

        if (integrationEvent is null)
        {
            throw new InvalidOperationException(
                "Unable to deserialize PostPublishedIntegrationEvent.");
        }

        logger.LogInformation(
            "Post published integration event received. PostId: {PostId}, AuthorUserId: {AuthorUserId}",
            integrationEvent.PostId,
            integrationEvent.AuthorUserId);

        dbContext.InboxMessages.Add(
            new InboxMessage(
                messageId,
                DateTimeOffset.UtcNow));

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}