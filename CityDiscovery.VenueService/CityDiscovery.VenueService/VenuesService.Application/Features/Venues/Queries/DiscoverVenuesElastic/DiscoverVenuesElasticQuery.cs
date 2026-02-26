using MediatR;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;

namespace CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.DiscoverVenuesElastic
{
    public class DiscoverVenuesElasticQuery : IRequest<List<VenueBasicDto>>
    {
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? VenueCategory { get; set; }
        public int? PriceLevel { get; set; }
        public bool? IsActive { get; set; }
        public double? MinAverageRating { get; set; }
    }
}