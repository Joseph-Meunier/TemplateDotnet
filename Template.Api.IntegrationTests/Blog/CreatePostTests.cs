using System.Net;
using System.Net.Http.Json;
using Template.Api.IntegrationTests.Infrastructure;
using Template.Modules.Users.Contracts;

namespace Template.Api.IntegrationTests.Blog;

public sealed class CreatePostTests(
    ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client =
        factory.CreateClient();

    [Fact]
    public async Task CreatePost_WithoutAuthentication_ReturnsUnauthorized()
    {
        var request = CreateValidRequest();

        var response = await _client.PostAsJsonAsync(
            "/blog/posts",
            request);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task CreatePost_WithAuthenticatedUserWithoutCreatorRole_ReturnsForbidden()
    {
        var identityId = $"user-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityId);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/posts")
        {
            Content = JsonContent.Create(
                CreateValidRequest())
        };

        request.Headers.Add(
            "X-Test-Identity",
            identityId);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task CreatePost_WithCreatorRole_ReturnsCreated()
    {
        var identityId = $"creator-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityId,
            roles: [UserRole.Creator]);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/posts")
        {
            Content = JsonContent.Create(
                CreateValidRequest())
        };

        request.Headers.Add(
            "X-Test-Identity",
            identityId);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }
    
    [Fact]
    public async Task CreatePost_WithoutApplicationProfile_ReturnsForbidden()
    {
        var identityId = $"unknown-{Guid.NewGuid()}";

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/posts")
        {
            Content = JsonContent.Create(
                CreateValidRequest())
        };

        request.Headers.Add(
            "X-Test-Identity",
            identityId);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    private static object CreateValidRequest()
    {
        return new
        {
            title = "Test post",
            description = "Test description",
            content = "# Test",
            startDate = "2026-08-24",
            heroImage = (string?)null,
            readingTimeMinutes = 5,
            tags = new[]
            {
                "dotnet",
                "testcontainers"
            }
        };
    }
}