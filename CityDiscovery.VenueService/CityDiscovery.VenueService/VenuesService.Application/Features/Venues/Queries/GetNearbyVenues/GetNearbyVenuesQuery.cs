using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;

public sealed record GetNearbyVenuesQuery(
    double Latitude,
    double Longitude,
    double RadiusInMeters
) : IRequest<List<NearbyVenueDto>>;

public sealed record NearbyVenueDto(
    Guid Id,
    string Name,
    double Latitude,
    double Longitude,
    double DistanceInMeters
);
