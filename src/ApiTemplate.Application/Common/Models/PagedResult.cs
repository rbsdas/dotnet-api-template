namespace ApiTemplate.Application.Common.Models;

/// <summary>Wraps a paged subset of results along with pagination metadata.</summary>
/// <typeparam name="T">The type of items in the result set.</typeparam>
public class PagedResult<T>
{
    /// <summary>Gets or sets the items in the current page.</summary>
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>Gets or sets the total number of items across all pages.</summary>
    public int TotalCount { get; init; }

    /// <summary>Gets or sets the current page number (1-based).</summary>
    public int Page { get; init; }

    /// <summary>Gets or sets the number of items per page.</summary>
    public int PageSize { get; init; }

    /// <summary>Gets the total number of pages.</summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Gets whether there is a page before the current one.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Gets whether there is a page after the current one.</summary>
    public bool HasNextPage => Page < TotalPages;
}
