using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Template.Modules.Blog.Features.PublishPost;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapPublishPostEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
                "/blog/posts/{id:guid}/publish",
                async (
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