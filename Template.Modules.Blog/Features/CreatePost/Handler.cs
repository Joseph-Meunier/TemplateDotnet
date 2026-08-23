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

        var post = new Post(
            request.AuthorUserId,
            request.Title,
            request.Description,
            request.Content,
            request.StartDate,
            request.HeroImage,
            request.ReadingTimeMinutes);

        dbContext.Posts.Add(post);

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return new Response(
            post.Id,
            post.Title);
    }
}