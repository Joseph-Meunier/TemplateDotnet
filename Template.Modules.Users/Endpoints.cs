using Microsoft.AspNetCore.Routing;
using Template.Modules.Users.Features.AddUserRole;
using Template.Modules.Users.Features.CreateUser;
using Template.Modules.Users.Features.DeleteUserRole;
using Template.Modules.Users.Features.GetUser;
using Template.Modules.Users.Features.RegisterUser;

namespace Template.Modules.Users;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapUsersModule(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapRegisterUserEndpoint();
        endpoints.MapCreateUserEndpoint();
        endpoints.MapGetUserEndpoint();
        endpoints.MapAddUserRole();
        endpoints.MapDeleteUserRoleEndpoint();

        return endpoints;
    }
}
