using CityDiscovery.Venues.Domain.ValueObjects;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.CreateVenue;

public sealed record CreateVenueCommand(
    Guid OwnerUserId,
    string Name,
    string? Description,
    string? AddressText,
    string? Phone,
    string? WebsiteUrl,
    byte? PriceLevel,
    string? OpeningHoursJson,
    double Latitude,
    double Longitude
) : IRequest<Guid>;
