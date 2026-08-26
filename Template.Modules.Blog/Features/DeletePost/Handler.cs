using Microsoft.EntityFrameworkCore;
using Template.Modules.Blog.Authorization;
using Template.Modules.Blog.Data;
using Template.Modules.Blog.Domain;
using Template.Modules.Users.Contracts;
using Template.Shared.Auth;
using Template.Shared.Errors;


namespace Template.Modules.Blog.Features.DeletePost;

public sealed class Handler(
    BlogDbContext dbContext,
    BlogAuthorizationService authorizationService)
{
    public async Task Handle(
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

        await authorizationService.RequireCanDeleteAsync(
            post,
            cancellationToken);

        dbContext.Posts.Remove(post);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}