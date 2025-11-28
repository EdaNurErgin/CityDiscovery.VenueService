using CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.SearchVenues;

public sealed class SearchVenuesQueryHandler
    : IRequestHandler<SearchVenuesQuery, IReadOnlyList<NearbyVenueDto>>
{
    private readonly IVenueRepository _venueRepository;

    public SearchVenuesQueryHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<IReadOnlyList<NearbyVenueDto>> Handle(
        SearchVenuesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _venueRepository.SearchVenuesAsync(
            request.Latitude,
            request.Longitude,
            request.RadiusInMeters,
            request.CategoryId,
            request.MinPriceLevel,
            request.MaxPriceLevel,
            request.OpenNow,
            cancellationToken);

        return result.AsReadOnly();
    }
}
