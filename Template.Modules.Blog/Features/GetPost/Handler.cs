using Microsoft.EntityFrameworkCore;
using Template.Modules.Blog.Data;
using Template.Shared.Errors;

namespace Template.Modules.Blog.Features.GetPost;

public sealed class Handler(
    BlogDbContext dbContext)
{
    public async Task<Response> Handle(
        Guid id,
        CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new Response(
                x.Id,
                x.AuthorUserId,
                x.Title,
                x.Description,
                x.Content,
                x.StartDate,
                x.PublishedAt,
                x.UpdatedAt,
                x.HeroImage,
                x.IsPublished,
                x.ReadingTimeMinutes,
                x.Tags
                    .OrderBy(t => t.Name)
                    .Select(t => t.Name)
                    .ToArray()))
            .SingleOrDefaultAsync(cancellationToken);

        if (post is null)
        {
            throw new NotFoundException(
                "blog.post_not_found",
                "The requested post does not exist.");
        }

        return post;
    }
}