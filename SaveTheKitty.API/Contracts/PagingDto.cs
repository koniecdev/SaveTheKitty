namespace SaveTheKitty.API.Contracts;
public class PagedList<T>(List<T> results, int totalCount, int? pageNumber = null, int? pageSize = null)
{
    public List<T> Results { get; set; } = results;
    public int PageNumber { get; set; } = pageNumber ?? 1;
    public int? PageSize { get; set; } = pageSize;
    public int TotalCount { get; set; } = totalCount;
    public bool HasNextPage => PageNumber * PageSize < TotalCount;
    public bool HasPreviousPage => PageSize > 1;
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int? page, int? pageSize, CancellationToken cancellationToken)
    {
        int totalCount = await query.CountAsync(cancellationToken);
        if(page > 0 && pageSize > 0)
        {
            query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }
        List<T> items = await query.ToListAsync(cancellationToken);
        return new(items, totalCount, page, pageSize);
    }
}
