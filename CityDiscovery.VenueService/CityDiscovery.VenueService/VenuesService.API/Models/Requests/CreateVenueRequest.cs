namespace CityDiscovery.Venues.API.Models.Requests;

public sealed class CreateVenueRequest
{
    public Guid OwnerUserId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? AddressText { get; set; }
    public string? Phone { get; set; }
    public string? WebsiteUrl { get; set; }
    public byte? PriceLevel { get; set; }
    public string? OpeningHoursJson { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
