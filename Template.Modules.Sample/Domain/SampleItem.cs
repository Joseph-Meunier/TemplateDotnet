namespace Template.Modules.Sample.Domain;

public sealed class SampleItem
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    private SampleItem()
    {
    }

    public SampleItem(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}