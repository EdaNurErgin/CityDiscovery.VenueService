using CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;
using CityDiscovery.Venues.Domain.Entities;

namespace CityDiscovery.Venues.Application.Interfaces.Repositories
{
    public interface IVenueRepository
    {
        Task<Venuex?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Venuex?> GetByOwnerIdAsync(Guid ownerUserId, CancellationToken cancellationToken = default);

        Task<bool> OwnerHasVenueAsync(Guid ownerUserId, CancellationToken cancellationToken = default);

        Task AddAsync(Venuex venue, CancellationToken cancellationToken = default);
        Task UpdateAsync(Venuex venue, CancellationToken cancellationToken = default);
        Task<List<NearbyVenueDto>> GetNearbyVenuesAsync(
            double latitude,
            double longitude,
            double radiusInMeters,
            CancellationToken cancellationToken = default);

    }
}
