using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateVenueBasicInfo;

public sealed record UpdateVenueBasicInfoCommand(
    Guid VenueId,
    string Name,
    string? Description,
    string? AddressText,
    string? Phone,
    string? WebsiteUrl,
    byte? PriceLevel,
    string? OpeningHoursJson,
    double Latitude,
    double Longitude
) : IRequest<Unit>;
