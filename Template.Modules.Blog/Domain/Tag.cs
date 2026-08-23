namespace Template.Modules.Blog.Domain;

public class Tag
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    private Tag()
    {
    }

    public Tag(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}