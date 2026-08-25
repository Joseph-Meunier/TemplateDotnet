using Microsoft.EntityFrameworkCore;
using Template.Modules.Blog.Data;
using Template.Shared.Pagination;

namespace Template.Modules.Blog.Features.GetPublishedPosts;

public sealed class Handler(
    BlogDbContext dbContext)
{
    public async Task<PagedResponse<PostItem>> Handle(
        Query request,
        CancellationToken cancellationToken)
    {
        var page = request.Page <= 0
            ? 1
            : request.Page;

        var pageSize = request.PageSize <= 0
            ? 10
            : Math.Min(request.PageSize, 100);

        var query = dbContext.Posts
            .AsNoTracking()
            .Where(x => x.IsPublished);

        if (!string.IsNullOrWhiteSpace(request.Tag))
        {
            var normalizedTag =
                request.Tag.Trim().ToLowerInvariant();

            query = query.Where(
                x => x.Tags.Any(
                    t => t.Name == normalizedTag));
        }

        var totalCount = await query.CountAsync(
            cancellationToken);

        var items = await query
            .OrderByDescending(x => x.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PostItem(
                x.Id,
                x.Title,
                x.Description,
                x.PublishedAt,
                x.HeroImage,
                x.ReadingTimeMinutes,
                x.Tags
                    .OrderBy(t => t.Name)
                    .Select(t => t.Name)
                    .ToArray()))
            .ToListAsync(cancellationToken);

        return new PagedResponse<PostItem>(
            items,
            page,
            pageSize,
            totalCount);
    }
}