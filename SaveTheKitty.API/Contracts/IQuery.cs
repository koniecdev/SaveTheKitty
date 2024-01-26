namespace SaveTheKitty.API.Contracts;

public interface ICollectionQuery
{
    public string? SortBy { get; init; }
    public string? SortOrder { get; init; }
    public int? Page { get; init; }
    public int? Size { get; init; }
}
