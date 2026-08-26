namespace Template.Modules.Blog.Features.UnpublishPost;

public sealed record Response(
    Guid Id,
    bool IsPublished,
    DateOnly? PublishedAt);