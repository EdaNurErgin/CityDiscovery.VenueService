namespace CityDiscovery.Venues.API.Models.Requests;

public sealed class AddVenuePhotoRequest
{
    public string Url { get; set; } = default!;
    public string? Caption { get; set; }
    public int? SortOrder { get; set; }
}
