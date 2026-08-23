using Microsoft.EntityFrameworkCore;
using Template.Modules.Sample.Domain;

namespace Template.Modules.Sample.Data;

public sealed class SampleDbContext(
    DbContextOptions<SampleDbContext> options)
    : DbContext(options)
{
    public DbSet<SampleItem> SampleItems => Set<SampleItem>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SampleDbContext).Assembly);
    }
}