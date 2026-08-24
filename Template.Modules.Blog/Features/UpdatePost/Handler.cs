using Microsoft.EntityFrameworkCore;
using Template.Modules.Blog.Data;
using Template.Modules.Blog.Domain;
using Template.Shared.Errors;

namespace Template.Modules.Blog.Features.UpdatePost;

public sealed class Handler(
    BlogDbContext dbContext)
{
    public async Task<Response> Handle(
        Guid id,
        Request request,
        CancellationToken cancellationToken)
    {
        var post = await dbContext.Posts
            .Include(x => x.Tags)
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (post is null)
        {
            throw new NotFoundException(
                "blog.post_not_found",
                "The requested post does not exist.");
        }

        post.Update(
            request.Title,
            request.Description,
            request.Content,
            request.StartDate,
            request.HeroImage,
            request.ReadingTimeMinutes);

        var normalizedTagNames = request.Tags
            .Select(x => x.Trim().ToLowerInvariant())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToArray();

        var existingTags = await dbContext.Tags
            .Where(x => normalizedTagNames.Contains(x.Name))
            .ToListAsync(cancellationToken);

        var existingNames = existingTags
            .Select(x => x.Name)
            .ToHashSet();

        var newTags = normalizedTagNames
            .Where(x => !existingNames.Contains(x))
            .Select(x => new Tag(x))
            .ToList();

        post.Tags.Clear();

        foreach (var tag in existingTags)
        {
            post.Tags.Add(tag);
        }

        foreach (var tag in newTags)
        {
            post.Tags.Add(tag);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new Response(
            post.Id,
            post.Title,
            post.UpdatedAt);
    }
}