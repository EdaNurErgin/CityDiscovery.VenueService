using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;

public sealed class GetNearbyVenuesQueryHandler
    : IRequestHandler<GetNearbyVenuesQuery, List<NearbyVenueDto>>
{
    private readonly IVenueRepository _venueRepository;

    public GetNearbyVenuesQueryHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<List<NearbyVenueDto>> Handle(GetNearbyVenuesQuery request, CancellationToken cancellationToken)
    {
        return await _venueRepository.GetNearbyVenuesAsync(
            request.Latitude,
            request.Longitude,
            request.RadiusInMeters,
            cancellationToken);
    }
}
