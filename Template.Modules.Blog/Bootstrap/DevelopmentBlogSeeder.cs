using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Template.Modules.Blog.Data;
using Template.Modules.Blog.Domain;
using Template.Modules.Users.Contracts;

namespace Template.Modules.Blog.Bootstrap;

public sealed class DevelopmentBlogSeeder(
    BlogDbContext blogDbContext,
    IUserReader userReader,
    IConfiguration configuration)
{
    public async Task RunAsync(
        CancellationToken cancellationToken = default)
    {
        var identityId = configuration[
            "BootstrapAdmin:IdentityId"];

        if (string.IsNullOrWhiteSpace(identityId))
        {
            return;
        }

        var author = await userReader.GetByIdentityIdAsync(
            identityId,
            cancellationToken);

        if (author is null)
        {
            return;
        }

        var alreadySeeded = await blogDbContext.Posts
            .AnyAsync(
                x => x.Title == "Getting started with the template",
                cancellationToken);

        if (alreadySeeded)
        {
            return;
        }

        var post1 = new Post(
            author.Id,
            "Getting started with the template",
            "A first example article.",
            """
            # Getting started

            This is a development seed article.

            It can be safely removed.
            """,
            DateOnly.FromDateTime(DateTime.UtcNow),
            null,
            3);

        var post2 = new Post(
            author.Id,
            "Modular monolith architecture",
            "Example article about modular architecture.",
            """
            # Modular monolith

            This article exists only as development data.

            Each module owns its data and exposes contracts
            to communicate with other modules.
            """,
            DateOnly.FromDateTime(DateTime.UtcNow),
            null,
            5);
        
        post2.Publish(DateOnly.FromDateTime(DateTime.UtcNow));
        post1.Publish(DateOnly.FromDateTime(DateTime.UtcNow));
        
        blogDbContext.Posts.AddRange(
            post1,
            post2);

        await blogDbContext.SaveChangesAsync(
            cancellationToken);
    }
}