using Microsoft.EntityFrameworkCore;
using Template.Modules.Blog.Authorization;
using Template.Modules.Blog.Data;
using Template.Shared.Errors;

namespace Template.Modules.Blog.Features.UnpublishPost;

public sealed class Handler(
    BlogDbContext dbContext,
    BlogAuthorizationService authorizationService)
{
    public async Task<Response> Handle(
        Guid id,
        CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (post is null)
        {
            throw new NotFoundException(
                "blog.post_not_found",
                "The requested post does not exist.");
        }

        await authorizationService.RequireCanPublishAsync(
            post,
            cancellationToken);

        post.Unpublish();

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return new Response(
            post.Id,
            post.IsPublished,
            post.PublishedAt);
    }
}