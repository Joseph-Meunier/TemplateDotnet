using Template.Modules.Users.Contracts;

namespace Template.Modules.Users.Domain;

public sealed class User
{
    public Guid Id { get; private set; }

    public string IdentityId { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string DisplayName { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }
    
    private readonly List<UserRoleAssignment> _roles = [];

    public IReadOnlyCollection<UserRoleAssignment> Roles => _roles;

    private User()
    {
    }

    public User(
        string identityId,
        string email,
        string displayName)
    {
        Id = Guid.NewGuid();

        IdentityId = identityId;
        Email = email.Trim().ToLowerInvariant();
        DisplayName = displayName.Trim();

        CreatedAt = DateTimeOffset.UtcNow;
    }

    public bool HasRole(UserRole role)
    {
        return _roles.Any(x => x.Role == role);
    }

    public void AddRole(UserRole role)
    {
        if (!HasRole(role))
        {
            _roles.Add(new UserRoleAssignment(Id, role));
        }
    }

    public void RemoveRole(UserRole role)
    {
        _roles.RemoveAll(x => x.Role == role);
    }
}