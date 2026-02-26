using CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.DiscoverVenuesElastic;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;

namespace CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services
{
    public interface IVenueSearchService
    {
        Task<bool> IndexVenueAsync(VenueBasicDto venue);
        Task<IEnumerable<VenueBasicDto>> SearchVenuesAsync(string keyword);
        
    }
}