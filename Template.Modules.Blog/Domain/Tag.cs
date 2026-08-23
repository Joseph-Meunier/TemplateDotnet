namespace Template.Modules.Blog.Domain;

public sealed class Tag
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public ICollection<Post> Posts { get; private set; } = [];

    private Tag()
    {
    }

    public Tag(string name)
    {
        Id = Guid.NewGuid();
        Name = name.Trim().ToLowerInvariant();
    }
}