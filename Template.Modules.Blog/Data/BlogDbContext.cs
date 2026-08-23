using Microsoft.EntityFrameworkCore;
using Template.Modules.Sample.Domain;

namespace Template.Modules.Blog.Data;

public sealed class BlogDbContext(
    DbContextOptions<BlogDbContext> options)
    : DbContext(options)
{
    public DbSet<Post> PostItems => Set<Post>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(BlogDbContext).Assembly);
    }
}