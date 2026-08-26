namespace Template.Shared.Auth;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    string IdentityId { get; }
}