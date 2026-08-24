namespace Template.Modules.Blog.Features.UpdatePost;

public sealed record Response(
    Guid Id,
    string Title,
    DateTimeOffset UpdatedAt);