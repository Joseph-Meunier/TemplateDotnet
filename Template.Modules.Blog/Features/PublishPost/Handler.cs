using Microsoft.EntityFrameworkCore;
using Template.Modules.Blog.Data;
using Template.Shared.Errors;

namespace Template.Modules.Blog.Features.PublishPost;

public sealed class Handler(
    BlogDbContext dbContext)
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

        post.Publish(
            DateOnly.FromDateTime(DateTime.UtcNow));

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return new Response(
            post.Id,
            post.IsPublished,
            post.PublishedAt);
    }
}