using CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.SearchVenues;

public sealed record SearchVenuesQuery(
    double Latitude,
    double Longitude,
    double RadiusInMeters,
    int? CategoryId,
    byte? MinPriceLevel,
    byte? MaxPriceLevel,
    bool? OpenNow
) : IRequest<IReadOnlyList<NearbyVenueDto>>;
