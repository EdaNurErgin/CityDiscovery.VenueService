namespace CityDiscovery.Venues.API.Models.Requests;

public sealed class CreateMenuItemRequest
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int? SortOrder { get; set; }
}
