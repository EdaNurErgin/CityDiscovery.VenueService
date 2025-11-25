using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Repositories;

public sealed class VenueCategoryRepository : IVenueCategoryRepository
{
    private readonly VenueDbContext _dbContext;

    public VenueCategoryRepository(VenueDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesForVenueAsync(
        Guid venueId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.VenueCategories
            .Where(vc => vc.VenueId == venueId)
            .Select(vc => vc.Category)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> VenueHasCategoryAsync(
        Guid venueId,
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.VenueCategories
            .AnyAsync(vc =>
                vc.VenueId == venueId &&
                vc.CategoryId == categoryId,
                cancellationToken);
    }

    public async Task AddCategoryToVenueAsync(
        Guid venueId,
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        if (await VenueHasCategoryAsync(venueId, categoryId, cancellationToken))
            return;

        var venueCategory = VenueCategory.Create(venueId, categoryId);

        _dbContext.VenueCategories.Add(venueCategory);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveCategoryFromVenueAsync(
        Guid venueId,
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.VenueCategories
            .FirstOrDefaultAsync(
                vc => vc.VenueId == venueId && vc.CategoryId == categoryId,
                cancellationToken);

        if (entity is null)
            return;

        _dbContext.VenueCategories.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
