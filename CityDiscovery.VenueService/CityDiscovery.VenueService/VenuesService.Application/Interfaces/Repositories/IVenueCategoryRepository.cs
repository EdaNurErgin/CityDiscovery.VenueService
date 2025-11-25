using CityDiscovery.Venues.Domain.Entities;

namespace CityDiscovery.Venues.Application.Interfaces.Repositories;

public interface IVenueCategoryRepository
{
    Task<IReadOnlyList<Category>> GetCategoriesForVenueAsync(
        Guid venueId,
        CancellationToken cancellationToken = default);

    Task<bool> VenueHasCategoryAsync(
        Guid venueId,
        int categoryId,
        CancellationToken cancellationToken = default);

    Task AddCategoryToVenueAsync(
        Guid venueId,
        int categoryId,
        CancellationToken cancellationToken = default);

    Task RemoveCategoryFromVenueAsync(
        Guid venueId,
        int categoryId,
        CancellationToken cancellationToken = default);
}
