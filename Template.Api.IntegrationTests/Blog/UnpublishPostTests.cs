using System.Net;
using System.Net.Http.Json;
using Template.Api.IntegrationTests.Infrastructure;
using static Template.Modules.Users.Contracts.UserRole;

namespace Template.Api.IntegrationTests.Blog;

public class UnpublishPostTests
(
    ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client =
        factory.CreateClient();

    // - Creator propriétaire -> 204
    [Fact]
    public async Task UnpublishPost_WithAuthenticatedUserWithCreatorRoleAndIsOwner_ReturnsOk()
    {
        var identityId = $"creator-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityId,
            roles: Creator);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/posts");
        
        request.Content = JsonContent.Create(
            CreateValidRequest());

        request.Headers.Add(
            "X-Test-Identity",
            identityId);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var createdPost =
            await response.Content
                .ReadFromJsonAsync<CreatePostResponse>();

        // Unpublish the post as the owner
        if (createdPost != null)
        {
            using var unpublishRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"/blog/posts/{createdPost.Id}/unpublish");

            unpublishRequest.Headers.Add(
                "X-Test-Identity",
                identityId);

            var unpublishResponse =
                await _client.SendAsync(unpublishRequest);

            Assert.Equal(
                HttpStatusCode.OK,
                unpublishResponse.StatusCode);
        }
    }

    // - Creator non propriétaire -> 403
    [Fact]
    public async Task UnpublishPost_WithAuthenticatedUserWithCreatorRoleAndIsNotOwner_ReturnsForbidden()
    {
        var identityIdCreator = $"creator-{Guid.NewGuid()}";
        var identityIdNotOwner = $"creator-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityIdCreator,
            roles: Creator);

        await factory.CreateUserAsync(
            identityId: identityIdNotOwner,
            roles: Creator);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/posts");
        
        request.Content = JsonContent.Create(
            CreateValidRequest());

        request.Headers.Add(
            "X-Test-Identity",
            identityIdCreator);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var createdPost =
            await response.Content
                .ReadFromJsonAsync<CreatePostResponse>();

        // Unpublish the post as the owner
        if (createdPost != null)
        {
            using var unpublishRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"/blog/posts/{createdPost.Id}/unpublish");

            unpublishRequest.Headers.Add(
                "X-Test-Identity",
                identityIdNotOwner);

            var unpublishResponse =
                await _client.SendAsync(unpublishRequest);  

            Assert.Equal(
                HttpStatusCode.Forbidden,
                unpublishResponse.StatusCode);
        }
    }
    
    // - Admin -> 204
    [Fact]
    public async Task PublishPost_WithAuthenticatedUserWithAdminRoleAndIsNotOwner_ReturnsOk()
    {
        var identityIdAdmin = $"admin-{Guid.NewGuid()}";
        var identityIdCreator = $"creator-{Guid.NewGuid()}";
        

        await factory.CreateUserAsync(
            identityId: identityIdCreator,
            roles: Creator);

        await factory.CreateUserAsync(
            identityId: identityIdAdmin,
            roles: Admin);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/posts");
        
        request.Content = JsonContent.Create(
            CreateValidRequest());

        request.Headers.Add(
            "X-Test-Identity",
            identityIdCreator);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var createdPost =
            await response.Content
                .ReadFromJsonAsync<CreatePostResponse>();

        // Publish the post as the admin
        if (createdPost != null)
        {
            using var unpublishRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"/blog/posts/{createdPost.Id}/unpublish");

            unpublishRequest.Headers.Add(
                "X-Test-Identity",
                identityIdAdmin);


            var unpublishResponse =
                await _client.SendAsync(unpublishRequest);

            Assert.Equal(
                HttpStatusCode.OK,
                unpublishResponse.StatusCode);
        }
    }
    
    private sealed record CreatePostResponse(
        Guid Id,
        string Title);
    
    private static object StartTags()
    {
        return new[]
        {
            "dotnet",
            "testcontainers"
        };
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
            tags = StartTags()
        };
    }
    
}
