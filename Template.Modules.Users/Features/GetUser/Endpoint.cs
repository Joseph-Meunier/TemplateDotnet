using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Template.Modules.Users.Features.GetUser;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetUserEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/users/{id:guid}", async (
                Guid id,
                Handler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.Handle(
                    id,
                    cancellationToken);

                return Results.Ok(response);
            })
            .Produces<Response>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }
}