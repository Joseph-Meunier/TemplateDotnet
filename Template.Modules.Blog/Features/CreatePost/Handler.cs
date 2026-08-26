using Microsoft.EntityFrameworkCore;
using Template.Modules.Blog.Authorization;
using Template.Modules.Blog.Data;
using Template.Modules.Blog.Domain;
using Template.Shared.Errors;

namespace Template.Modules.Blog.Features.CreatePost;

public sealed class Handler(
    BlogDbContext dbContext,
    BlogAuthorizationService authorizationService)
{
    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        var author = await authorizationService.RequireCreatorAsync(
            cancellationToken);

        if (author is null)
        {
            throw new NotFoundException(
                "users.current_user_not_found",
                "The authenticated user has no application profile.");
        }

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
        
        var post = new Post(
            author.Id,
            request.Title,
            request.Description,
            request.Content,
            request.StartDate,
            request.HeroImage,
            request.ReadingTimeMinutes);

        foreach (var tag in existingTags)
        {
            post.Tags.Add(tag);
        }

        foreach (var tag in newTags)
        {
            post.Tags.Add(tag);
        }

        dbContext.Posts.Add(post);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new Response(
            post.Id,
            post.Title);
    }
}