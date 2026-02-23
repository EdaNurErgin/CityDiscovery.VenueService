using MediatR;
using CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;

namespace CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.SearchVenuesElastic
{
    public class SearchVenuesElasticQueryHandler : IRequestHandler<SearchVenuesElasticQuery, List<VenueBasicDto>>
    {
        private readonly IVenueSearchService _venueSearchService;

        public SearchVenuesElasticQueryHandler(IVenueSearchService venueSearchService)
        {
            _venueSearchService = venueSearchService;
        }

        public async Task<List<VenueBasicDto>> Handle(SearchVenuesElasticQuery request, CancellationToken cancellationToken)
        {
            var results = await _venueSearchService.SearchVenuesAsync(request.Keyword);
            return results.ToList();
        }
    }
}