using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Template.Modules.Users.Contracts;
using Template.Shared.Validation;

namespace Template.Modules.Users.Features.DeleteUserRole;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapCreateUserEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/users/{id:guid}/roles/{role}", async (
                Guid id,
                UserRole role,
                Handler handler,
                CancellationToken cancellationToken) =>
            {
                await handler.Handle(
                    id,
                    role,
                    cancellationToken);

                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return endpoints;
    }
}