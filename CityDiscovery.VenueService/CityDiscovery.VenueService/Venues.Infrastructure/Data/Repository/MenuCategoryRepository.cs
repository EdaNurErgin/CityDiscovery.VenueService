using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Repository;

public sealed class MenuCategoryRepository : IMenuCategoryRepository
{
    private readonly VenueDbContext _context;

    public MenuCategoryRepository(VenueDbContext context)
    {
        _context = context;
    }

    public async Task<MenuCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.MenuCategories
            .Include(mc => mc.Items)
            .FirstOrDefaultAsync(mc => mc.MenuCategoryId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MenuCategory>> GetByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        var list = await _context.MenuCategories
            .Include(mc => mc.Items)
            .Where(mc => mc.VenueId == venueId && mc.IsActive)
            .OrderBy(mc => mc.SortOrder)
            .ToListAsync(cancellationToken);

        return list.AsReadOnly();
    }

    public async Task AddAsync(MenuCategory category, CancellationToken cancellationToken = default)
    {
        await _context.MenuCategories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MenuCategory category, CancellationToken cancellationToken = default)
    {
        _context.MenuCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MenuCategory category, CancellationToken cancellationToken = default)
    {
        _context.MenuCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
