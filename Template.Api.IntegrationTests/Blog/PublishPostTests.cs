using System.Net;
using System.Net.Http.Json;
using Template.Api.IntegrationTests.Infrastructure;
using Template.Modules.Users.Contracts;

namespace Template.Api.IntegrationTests.Blog;

public class PublishPostTests(
    ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client =
        factory.CreateClient();

    // - Creator propriétaire -> 204
    [Fact]
    public async Task PublishPost_WithAuthenticatedUserWithCreatorRoleAndIsOwner_ReturnsOk()
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

        var createdPost =
            await response.Content
                .ReadFromJsonAsync<CreatePostResponse>();

        // Publish the post as the owner
        using var publishRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/blog/posts/{createdPost.Id}/publish");

        publishRequest.Headers.Add(
            "X-Test-Identity",
            identityId);

        var publishResponse =
            await _client.SendAsync(publishRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            publishResponse.StatusCode);
    }

    // - Creator non propriétaire -> 403
    [Fact]
    public async Task PublishPost_WithAuthenticatedUserWithCreatorRoleAndIsNotOwner_ReturnsForbidden()
    {
        var identityIdCreator = $"creator-{Guid.NewGuid()}";
        var identityIdNotOwner = $"creator-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityIdCreator,
            roles: [UserRole.Creator]);

        await factory.CreateUserAsync(
            identityId: identityIdNotOwner,
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
            identityIdCreator);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var createdPost =
            await response.Content
                .ReadFromJsonAsync<CreatePostResponse>();

        // Publish the post as the owner
        using var publishRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/blog/posts/{createdPost.Id}/publish");

        publishRequest.Headers.Add(
            "X-Test-Identity",
            identityIdNotOwner);

        var publishResponse =
            await _client.SendAsync(publishRequest);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            publishResponse.StatusCode);
    }
    
    // - Admin -> 204
    [Fact]
    public async Task PublishPost_WithAuthenticatedUserWithAdminRoleAndIsNotOwner_ReturnsOk()
    {
        var identityIdAdmin = $"admin-{Guid.NewGuid()}";
        var identityIdCreator = $"creator-{Guid.NewGuid()}";
        

        await factory.CreateUserAsync(
            identityId: identityIdCreator,
            roles: [UserRole.Creator]);

        await factory.CreateUserAsync(
            identityId: identityIdAdmin,
            roles: [UserRole.Admin]);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/posts")
        {
            Content = JsonContent.Create(
                CreateValidRequest())
        };

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
        using var publishRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/blog/posts/{createdPost.Id}/publish");

        publishRequest.Headers.Add(
            "X-Test-Identity",
            identityIdAdmin);

        var publishResponse =
            await _client.SendAsync(publishRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            publishResponse.StatusCode);
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