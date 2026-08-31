namespace Template.Modules.Users.Contracts;

public interface IIdentityProvider
{
    Task<string> CreateUserAsync(
        string email,
        string displayName,
        string password,
        CancellationToken cancellationToken);
}
