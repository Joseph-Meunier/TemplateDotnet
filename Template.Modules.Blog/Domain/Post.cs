using Template.Modules.Blog.Events;
using Template.Shared.Events;

namespace Template.Modules.Blog.Domain;

public sealed class Post
{
    public Guid Id { get; private set; }

    public Guid AuthorUserId { get; private set; }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public string Content { get; private set; } = null!;

    public DateOnly StartDate { get; private set; }

    public DateOnly? PublishedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public string? HeroImage { get; private set; }

    public bool IsPublished { get; private set; }

    public int ReadingTimeMinutes { get; private set; }
    
    public ICollection<Tag> Tags { get; private set; } = [];

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents
        => _domainEvents.AsReadOnly();
    
    private void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    
    private Post()
    {
    }

    public Post(
        Guid authorUserId,
        string title,
        string description,
        string content,
        DateOnly startDate,
        string? heroImage,
        int readingTimeMinutes)
    {
        Id = Guid.NewGuid();
        AuthorUserId = authorUserId;
        Title = title.Trim();
        Description = description.Trim();
        Content = content;

        StartDate = startDate;

        HeroImage = heroImage;

        ReadingTimeMinutes = readingTimeMinutes;

        IsPublished = false;

        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void Publish(DateOnly publicationDate)
    {
        if (IsPublished)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        
        IsPublished = true;
        PublishedAt = publicationDate;
        UpdatedAt = now;
        RaiseDomainEvent
        (
            new PostPublishedDomainEvent
            (
                Id,
                AuthorUserId,
                now
            )
        );
    
    }
    
    public void Unpublish()
    {
        if (!IsPublished)
        {
            return;
        }

        IsPublished = false;
        PublishedAt = null;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void Update(
        string title,
        string description,
        string content,
        DateOnly startDate,
        string? heroImage,
        int readingTimeMinutes)
    {
        Title = title.Trim();
        Description = description.Trim();
        Content = content;
        StartDate = startDate;
        HeroImage = heroImage;
        ReadingTimeMinutes = readingTimeMinutes;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}