namespace Template.Modules.Users.Domain;

public sealed class UserRoleAssignment
{
    public Guid UserId { get; private set; }

    public Contracts.UserRole Role { get; private set; }

    private UserRoleAssignment()
    {
    }

    internal UserRoleAssignment(
        Guid userId,
        Contracts.UserRole role)
    {
        UserId = userId;
        Role = role;
    }
    
    
}