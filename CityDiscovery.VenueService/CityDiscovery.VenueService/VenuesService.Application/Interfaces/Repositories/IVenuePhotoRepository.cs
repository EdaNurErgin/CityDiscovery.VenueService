using CityDiscovery.Venues.Domain.Entities;

namespace CityDiscovery.Venues.Application.Interfaces.Repositories;

public interface IVenuePhotoRepository
{
    Task<VenuePhoto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VenuePhoto>> GetByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default);
    Task AddAsync(VenuePhoto photo, CancellationToken cancellationToken = default);
    Task RemoveAsync(VenuePhoto photo, CancellationToken cancellationToken = default);
}
