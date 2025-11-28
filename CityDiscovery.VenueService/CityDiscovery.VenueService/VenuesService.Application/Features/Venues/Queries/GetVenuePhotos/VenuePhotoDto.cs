namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenuePhotos;

public sealed class VenuePhotoDto
{
    public Guid Id { get; init; }
    public string Url { get; init; } = default!;
    public string? Caption { get; init; }
    public int SortOrder { get; init; }
}
