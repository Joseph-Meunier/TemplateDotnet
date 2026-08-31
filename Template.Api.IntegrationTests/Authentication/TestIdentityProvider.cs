using Template.Modules.Users.Contracts;

namespace Template.Api.IntegrationTests.Authentication;

public sealed class TestIdentityProvider : IIdentityProvider
{
    public Task<string> CreateUserAsync(
        string email,
        string displayName,
        string password,
        CancellationToken cancellationToken)
    {
        return Task.FromResult($"keycloak-{Guid.NewGuid()}");
    }
}
