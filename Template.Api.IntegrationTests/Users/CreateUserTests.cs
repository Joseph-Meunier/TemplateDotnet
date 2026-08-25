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
    public async Task CreateUser_WithValidRequest_ReturnsCreated()
    {
        var request = new
        {
            email = "john@example.com",
            displayName = "John"
        };

        var response = await _client.PostAsJsonAsync(
            "/users",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }
    
    [Fact]
    public async Task CreateUser_WithExistingEmail_ReturnsConflict()
    {
        var request = new
        {
            email = "duplicate@example.com",
            displayName = "User"
        };

        var firstResponse =
            await _client.PostAsJsonAsync(
                "/users",
                request);

        Assert.Equal(
            HttpStatusCode.Created,
            firstResponse.StatusCode);

        var secondResponse =
            await _client.PostAsJsonAsync(
                "/users",
                request);

        Assert.Equal(
            HttpStatusCode.Conflict,
            secondResponse.StatusCode);
    }
}
