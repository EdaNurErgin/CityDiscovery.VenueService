namespace CityDiscovery.Venues.API.Models.Requests;

public sealed class CreateMenuCategoryRequest
{
    public string Name { get; set; } = default!;
    public int? SortOrder { get; set; }
}
