namespace Template.Shared.Pagination;

public sealed record PagedRequest(
    int Page = 1,
    int PageSize = 10);