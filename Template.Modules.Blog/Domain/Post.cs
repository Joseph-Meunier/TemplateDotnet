using Template.Modules.Blog.Domain;

public sealed class Post
{
    public Guid Id { get; private set; }

    public Guid AuthorUserId { get; private set; }

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Content { get; private set; } = null!;

    public DateOnly StartDate { get; private set; }
    public DateOnly PubDate { get; private set; }
    public DateOnly UpdatedDate { get; private set; }

    public string? HeroImage { get; private set; }

    public bool IsPublished { get; private set; }

    public int ReadingTimeMinutes { get; private set; }

    public ICollection<Tag> Tags { get; private set; } = [];
}