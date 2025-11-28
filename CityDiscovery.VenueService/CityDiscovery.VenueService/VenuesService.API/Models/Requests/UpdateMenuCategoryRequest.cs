namespace CityDiscovery.Venues.API.Models.Requests;

public sealed class UpdateMenuCategoryRequest
{
    public string Name { get; set; } = default!;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}
