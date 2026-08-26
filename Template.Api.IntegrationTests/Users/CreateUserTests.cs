using System.Net;
using System.Net.Http.Json;
using Template.Api.IntegrationTests.Infrastructure;

namespace Template.Api.IntegrationTests.Users;

public sealed class CreateUserTests(
    ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client =
        factory.CreateClient();

    [Fact]
    public async Task CreateUser_WithoutAuthentication_ReturnsUnauthorized()
    {
        var request = new
        {
            email = $"john-{Guid.NewGuid()}@example.com",
            displayName = "John"
        };

        var response = await _client.PostAsJsonAsync(
            "/users",
            request);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithValidRequest_ReturnsCreated()
    {
        var identityId = $"identity-{Guid.NewGuid()}";

        var response = await SendAuthenticatedAsync(
            identityId,
            new
            {
                email = $"john-{Guid.NewGuid()}@example.com",
                displayName = "John"
            });

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithExistingEmail_ReturnsConflict()
    {
        var email =
            $"duplicate-{Guid.NewGuid()}@example.com";

        var firstResponse = await SendAuthenticatedAsync(
            $"identity-{Guid.NewGuid()}",
            new
            {
                email,
                displayName = "First user"
            });

        Assert.Equal(
            HttpStatusCode.Created,
            firstResponse.StatusCode);

        var secondResponse = await SendAuthenticatedAsync(
            $"identity-{Guid.NewGuid()}",
            new
            {
                email,
                displayName = "Second user"
            });

        Assert.Equal(
            HttpStatusCode.Conflict,
            secondResponse.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithExistingIdentity_ReturnsConflict()
    {
        var identityId =
            $"identity-{Guid.NewGuid()}";

        var firstResponse = await SendAuthenticatedAsync(
            identityId,
            new
            {
                email = $"first-{Guid.NewGuid()}@example.com",
                displayName = "First user"
            });

        Assert.Equal(
            HttpStatusCode.Created,
            firstResponse.StatusCode);

        var secondResponse = await SendAuthenticatedAsync(
            identityId,
            new
            {
                email = $"second-{Guid.NewGuid()}@example.com",
                displayName = "Second user"
            });

        Assert.Equal(
            HttpStatusCode.Conflict,
            secondResponse.StatusCode);
    }

    private async Task<HttpResponseMessage> SendAuthenticatedAsync(
        string identityId,
        object body)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/users")
        {
            Content = JsonContent.Create(body)
        };

        request.Headers.Add(
            "X-Test-Identity",
            identityId);

        return await _client.SendAsync(request);
    }
}