using MediatR;
using CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;

namespace CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.DiscoverVenuesElastic
{
    public class DiscoverVenuesElasticQueryHandler : IRequestHandler<DiscoverVenuesElasticQuery, List<VenueBasicDto>>
    {
        private readonly ICityDiscoveryService _cityDiscoveryService;

        public DiscoverVenuesElasticQueryHandler(ICityDiscoveryService cityDiscoveryService)
        {
            _cityDiscoveryService = cityDiscoveryService;
        }

        public async Task<List<VenueBasicDto>> Handle(DiscoverVenuesElasticQuery request, CancellationToken cancellationToken)
        {
            var results = await _cityDiscoveryService.DiscoverVenuesAsync(request);
            return results.ToList();
        }
    }
}