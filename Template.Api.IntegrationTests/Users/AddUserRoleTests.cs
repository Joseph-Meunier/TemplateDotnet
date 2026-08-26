using System.Net;
using System.Net.Http.Json;
using Template.Api.IntegrationTests.Infrastructure;
using Template.Modules.Users.Contracts;

namespace Template.Api.IntegrationTests.Users;

public class AddUserRoleTests(
    ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client =
        factory.CreateClient();

    // - Admin -> 204
    [Fact]
    public async Task AddUserRole_WithAdminRole_ReturnsNoContent()
    {
        var identityIdAdmin = $"admin-{Guid.NewGuid()}";
        var identityIdUser = $"user-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityIdAdmin,
            roles: [UserRole.Admin]);

        var user = await factory.CreateUserAsync(
            identityId: identityIdUser,
            roles: []);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/users/{user.Id}/roles/{UserRole.Admin}");

        request.Headers.Add(
            "X-Test-Identity",
            identityIdAdmin);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }


    // - non Admin -> 403
    [Fact]
    public async Task AddUserRole_WithNonAdminRole_ReturnsForbidden()
    {
        var identityIdUser1 = $"user1-{Guid.NewGuid()}";
        var identityIdUser2 = $"user2-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityIdUser1,
            roles: []);

        var user = await factory.CreateUserAsync(
            identityId: identityIdUser2,
            roles: []);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/users/{user.Id}/roles/{UserRole.Admin}");

        request.Headers.Add(
            "X-Test-Identity",
            identityIdUser1);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}