using MediatR;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;

namespace CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.SearchVenuesElastic
{
    public class SearchVenuesElasticQuery : IRequest<List<VenueBasicDto>>
    {
        public string Keyword { get; set; }
    }
}