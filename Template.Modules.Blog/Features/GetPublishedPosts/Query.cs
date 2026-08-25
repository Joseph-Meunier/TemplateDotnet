using Microsoft.AspNetCore.Http;
using Template.Shared.Pagination;

namespace Template.Modules.Blog.Features.GetPublishedPosts;

public sealed record Query(
    int Page = 1,
    int PageSize = 10,
    string? Tag = null);