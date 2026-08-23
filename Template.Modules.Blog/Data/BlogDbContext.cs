using Microsoft.EntityFrameworkCore;
using Template.Modules.Blog.Domain;

namespace Template.Modules.Blog.Data;

public sealed class BlogDbContext(
    DbContextOptions<BlogDbContext> options)
    : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();

    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(BlogDbContext).Assembly);
    }
}