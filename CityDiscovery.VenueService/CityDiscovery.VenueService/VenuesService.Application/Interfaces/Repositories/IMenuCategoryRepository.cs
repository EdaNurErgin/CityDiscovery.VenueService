using CityDiscovery.Venues.Domain.Entities;

namespace CityDiscovery.Venues.Application.Interfaces.Repositories;

public interface IMenuCategoryRepository
{
    Task<MenuCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(MenuCategory category, CancellationToken cancellationToken = default);
}
