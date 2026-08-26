using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Template.Shared.Validation;

namespace Template.Modules.Users.Features.AddUserRole;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapCreateUserEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/users/{id:guid}/role", async (
                Guid id,
                Request request,
                Handler handler,
                CancellationToken cancellationToken) =>
            {
                await handler.Handle(
                    id,
                    request,
                    cancellationToken);

                return Results.Ok();
            })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return endpoints;
    }
}