using Microsoft.AspNetCore.Routing;
using Template.Modules.Blog.Features.CreatePost;
using Template.Modules.Blog.Features.GetPost;
using Template.Modules.Blog.Features.GetPublishedPosts;
using Template.Modules.Blog.Features.PublishPost;
using Template.Modules.Blog.Features.UpdatePost;

namespace Template.Modules.Blog;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapBlogModule(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCreatePostEndpoint();
        endpoints.MapGetPostEndpoint();
        endpoints.MapPublishPostEndpoint();
        endpoints.MapUpdatePostEndpoint();
        endpoints.MapGetPublishedPostsEndpoint();
        
        return endpoints;
    }
}