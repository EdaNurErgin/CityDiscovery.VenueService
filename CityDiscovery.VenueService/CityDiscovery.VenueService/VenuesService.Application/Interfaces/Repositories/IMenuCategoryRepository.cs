using CityDiscovery.Venues.Domain.Entities;

namespace CityDiscovery.Venues.Application.Interfaces.Repositories;

public interface IMenuCategoryRepository
{
    Task<MenuCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(MenuCategory category, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuCategory>> GetByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default);
    Task UpdateAsync(MenuCategory category, CancellationToken cancellationToken = default);
    Task DeleteAsync(MenuCategory category, CancellationToken cancellationToken = default);

}
