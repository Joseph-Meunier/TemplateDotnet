using Microsoft.EntityFrameworkCore;
using Template.Modules.Blog.Data;
using Template.Modules.Blog.Domain;
using Template.Modules.Users.Contracts;
using Template.Shared.Errors;

namespace Template.Modules.Blog.Features.CreatePost;

public sealed class Handler(
    BlogDbContext dbContext,
    IUserReader userReader)
{
    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        var authorExists = await userReader.ExistsAsync(
            request.AuthorUserId,
            cancellationToken);

        if (!authorExists)
        {
            throw new NotFoundException(
                "users.not_found",
                "The requested author does not exist.");
        }

        var normalizedTagNames = request.Tags
            .Select(x => x.Trim().ToLowerInvariant())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToArray();

        var existingTags = await dbContext.Tags
            .Where(x => normalizedTagNames.Contains(x.Name))
            .ToListAsync(cancellationToken);

        var existingTagNames = existingTags
            .Select(x => x.Name)
            .ToHashSet();

        var newTags = normalizedTagNames
            .Where(x => !existingTagNames.Contains(x))
            .Select(x => new Tag(x))
            .ToList();

        var post = new Post(
            request.AuthorUserId,
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

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return new Response(
            post.Id,
            post.Title);
    }
}