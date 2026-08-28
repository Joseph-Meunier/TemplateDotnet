using Template.Modules.Blog.Contracts.Events;
using Template.Shared.Events;

namespace Template.Modules.Blog.Events;

internal static class BlogIntegrationEventMapper
{
    public static object? Map(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            PostPublishedDomainEvent e =>
                new PostPublishedIntegrationEvent(
                    e.PostId,
                    e.AuthorUserId,
                    e.OccurredAt),

            _ => null
        };
    }
}