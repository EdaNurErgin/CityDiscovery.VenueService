namespace CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;

public sealed class VenueDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public Guid OwnerUserId { get; init; }
    public LocationDto Location { get; init; } = default!;
    public byte? PriceLevel { get; init; }
    public string? AddressText { get; init; }
    public string? Phone { get; init; }
    public string? WebsiteUrl { get; init; }
    public string? OpeningHoursJson { get; init; }
    public bool IsApproved { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}