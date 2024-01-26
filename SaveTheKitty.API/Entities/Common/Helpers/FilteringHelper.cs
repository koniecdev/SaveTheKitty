namespace SaveTheKitty.API.Entities.Common.Helpers;
public static class FilteringHelper
{
    public static (FiltersMethods method, string value)? MapFilter(string[] propsToFilter, string searchedProperty)
    {
        string[]? splittedProperty = propsToFilter
            .FirstOrDefault(m => m.StartsWith(searchedProperty.ToLower(), StringComparison.CurrentCultureIgnoreCase))
            ?.Split(':').ToArray();
        if (splittedProperty is null)
        {
            return null;
        }
        FiltersMethods methodToReturn = splittedProperty[1] switch
        {
            "eq" => FiltersMethods.filter_eq,
            "in" => FiltersMethods.filter_in,
            "gte" => FiltersMethods.filter_gte,
            "lte" => FiltersMethods.filter_lte,
            _ => FiltersMethods.filter_eq
        };
        return (methodToReturn, splittedProperty[2]);
    }
    public static (FiltersMethods method, string value)? MapFilter(string[] propsToFilter, string searchedProperty, FiltersMethods searchedFilter)
    {
        string[]? splittedProperty = propsToFilter
            .FirstOrDefault(m => m.StartsWith($"{searchedProperty.ToLower()}:{searchedFilter.ToString().Replace("filter_", "")}", StringComparison.CurrentCultureIgnoreCase))
            ?.Split(':').ToArray();
        if (splittedProperty is null)
        {
            return null;
        }
        return (searchedFilter, splittedProperty[2]);
    }
}