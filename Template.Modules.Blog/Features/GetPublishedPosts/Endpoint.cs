using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Template.Shared.Pagination;

namespace Template.Modules.Blog.Features.GetPublishedPosts;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetPublishedPostsEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
                "/blog/posts",
                async (
                    [AsParameters] Query query,
                    Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    var response = await handler.Handle(
                        query,
                        cancellationToken);

                    return Results.Ok(response);
                })
            .Produces<PagedResponse<PostItem>>(
                StatusCodes.Status200OK);

        return endpoints;
    }
}