using Template.Modules.Blog.Contracts.Events;
using Template.Shared.Events;

namespace Template.Modules.Blog.Messaging;

internal sealed class BlogMessageTopology
    : IMessageTopology
{
    public IReadOnlyCollection<MessageSubscription> Subscriptions { get; } =
    [
        new(
            "template.blog.post-published",
            typeof(PostPublishedIntegrationEvent).FullName!)
    ];
}