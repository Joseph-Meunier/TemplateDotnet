namespace Template.Modules.Users.Domain;

public sealed class User
{
    public Guid Id { get; private set; }

    public string Email { get; private set; } = null!;

    public string DisplayName { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    private User()
    {
    }

    public User(
        string email,
        string displayName)
    {
        Id = Guid.NewGuid();

        Email = email.Trim().ToLowerInvariant();
        DisplayName = displayName.Trim();

        CreatedAt = DateTimeOffset.UtcNow;
    }
}