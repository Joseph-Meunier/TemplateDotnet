using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Template.Modules.Blog.Data.Outbox;
using Template.Modules.Blog.Domain;
using Template.Modules.Blog.Events;
using Template.Shared.Events;
using Template.Modules.Blog.Data.Inbox;

namespace Template.Modules.Blog.Data;

public sealed class BlogDbContext(
    DbContextOptions<BlogDbContext> options)
    : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(BlogDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var domainEvents = GetDomainEvents();

        foreach (var domainEvent in domainEvents)
        {
            var integrationEvent =
                BlogIntegrationEventMapper.Map(domainEvent);

            if (integrationEvent is null)
            {
                continue;
            }

            var outboxMessage = new OutboxMessage(
                Guid.NewGuid(),
                integrationEvent.GetType().FullName!,
                JsonSerializer.Serialize(
                    integrationEvent,
                    integrationEvent.GetType()),
                domainEvent.OccurredAt);

            OutboxMessages.Add(outboxMessage);
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        ClearDomainEvents();

        return result;
    }

    private List<IDomainEvent> GetDomainEvents()
    {
        return ChangeTracker
            .Entries<Post>()
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();
    }

    private void ClearDomainEvents()
    {
        foreach (var entry in ChangeTracker.Entries<Post>())
        {
            entry.Entity.ClearDomainEvents();
        }
    }
}