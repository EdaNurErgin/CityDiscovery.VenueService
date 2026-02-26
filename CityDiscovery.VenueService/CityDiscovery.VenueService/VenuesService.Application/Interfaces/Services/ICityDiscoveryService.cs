using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.DiscoverVenuesElastic;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services
{
    public interface ICityDiscoveryService
    {
        Task<IEnumerable<VenueBasicDto>> DiscoverVenuesAsync(DiscoverVenuesElasticQuery query);
        Task<bool> IndexVenueWithDetailsAsync(Venuex venue);
    }
}