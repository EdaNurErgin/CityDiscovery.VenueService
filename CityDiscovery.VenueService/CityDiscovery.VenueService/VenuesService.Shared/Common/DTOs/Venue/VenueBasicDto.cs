namespace CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;

/// <summary>
/// Post'larda ve feed'lerde sadece temel mekan bilgisini göstermek için
/// </summary>
public sealed class VenueBasicDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? AddressText { get; init; }
    public byte? PriceLevel { get; init; }
    public LocationDto? Location { get; init; }
}