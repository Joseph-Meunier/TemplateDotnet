namespace Template.Modules.Blog.Features.UpdatePost;

public sealed record Request(
    string Title,
    string Description,
    string Content,
    DateOnly StartDate,
    string? HeroImage,
    int ReadingTimeMinutes,
    IReadOnlyCollection<string> Tags);