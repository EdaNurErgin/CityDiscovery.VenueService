using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly VenueDbContext _dbContext;

    public CategoryRepository(VenueDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Category>> GetAllActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .AnyAsync(c => c.CategoryId == id && c.IsActive, cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _dbContext.Categories.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken); // INT IDENTITY olduğu için kaydetmek şart, böylece ID oluşur
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        // Aynı isimde kategori var mı diye veritabanında kontrol et
        return await _dbContext.Categories.AnyAsync(c => c.Name == name, cancellationToken);
    }
}
