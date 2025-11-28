using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Repository;

public sealed class MenuItemRepository : IMenuItemRepository
{
    private readonly VenueDbContext _context;

    public MenuItemRepository(VenueDbContext context)
    {
        _context = context;
    }

    public async Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems
            .FirstOrDefaultAsync(mi => mi.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> GetByMenuCategoryIdAsync(int menuCategoryId, CancellationToken cancellationToken = default)
    {
        var list = await _context.MenuItems
            .Where(mi => mi.MenuCategoryId == menuCategoryId)
            .OrderBy(mi => mi.SortOrder)
            .ToListAsync(cancellationToken);

        return list.AsReadOnly();
    }

    public async Task AddAsync(MenuItem item, CancellationToken cancellationToken = default)
    {
        await _context.MenuItems.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MenuItem item, CancellationToken cancellationToken = default)
    {
        _context.MenuItems.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MenuItem item, CancellationToken cancellationToken = default)
    {
        _context.MenuItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
