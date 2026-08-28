using Template.Shared.Events;

namespace Template.Modules.Blog.Events;

public sealed record PostPublishedDomainEvent(
    Guid PostId,
    Guid AuthorUserId,
    DateTimeOffset OccurredAt)
    : IDomainEvent;