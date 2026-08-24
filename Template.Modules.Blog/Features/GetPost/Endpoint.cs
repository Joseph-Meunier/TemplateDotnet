using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Template.Modules.Blog.Features.GetPost;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetPostEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/blog/posts/{id:guid}", async (
                Guid id,
                Handler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.Handle(
                    id,
                    cancellationToken);

                return Results.Ok(response);
            })
            .Produces<Response>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }
}