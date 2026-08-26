using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Template.Shared.Validation;

namespace Template.Modules.Blog.Features.DeletePost;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapDeletePostEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/blog/post/{id:guid}", async (
                Guid id,
                Handler handler,
                CancellationToken cancellationToken) =>
            {
                await handler.Handle(
                    id,
                    cancellationToken);

                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return endpoints;
    }
}