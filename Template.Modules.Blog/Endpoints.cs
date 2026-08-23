using Microsoft.AspNetCore.Routing;
using Template.Modules.Blog.Features.CreatePost;

namespace Template.Modules.Blog;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapBlogModule(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCreatePostEndpoint();

        return endpoints;
    }
}