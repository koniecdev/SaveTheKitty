namespace SaveTheKitty.API.Contracts;

public interface IQuery
{
    public string? SortBy { get; init; }
    public string? SortOrder { get; init; }
    public int? Page { get; init; }
    public int? Size { get; init; }
}
