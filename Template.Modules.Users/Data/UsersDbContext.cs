using Microsoft.EntityFrameworkCore;
using Template.Modules.Users.Domain;

namespace Template.Modules.Users.Data;

public sealed class UsersDbContext(
    DbContextOptions<UsersDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(UsersDbContext).Assembly);
    }
}