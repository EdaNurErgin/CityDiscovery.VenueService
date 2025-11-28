using CityDiscovery.Venues.Domain.Entities;

namespace CityDiscovery.Venues.Application.Interfaces.Repositories;

public interface IMenuItemRepository
{
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(MenuItem item, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItem>> GetByMenuCategoryIdAsync(int menuCategoryId, CancellationToken cancellationToken = default);
    Task UpdateAsync(MenuItem item, CancellationToken cancellationToken = default);
    Task DeleteAsync(MenuItem item, CancellationToken cancellationToken = default);

}
