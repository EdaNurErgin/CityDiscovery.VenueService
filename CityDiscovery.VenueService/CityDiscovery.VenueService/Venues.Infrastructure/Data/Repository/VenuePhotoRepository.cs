using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Repository;

public sealed class VenuePhotoRepository : IVenuePhotoRepository
{
    private readonly VenueDbContext _context;

    public VenuePhotoRepository(VenueDbContext context)
    {
        _context = context;
    }

    public async Task<VenuePhoto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.VenuePhotos
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<VenuePhoto>> GetByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        var list = await _context.VenuePhotos
            .Where(p => p.VenueId == venueId)
            .OrderBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);

        return list.AsReadOnly();
    }

    public async Task AddAsync(VenuePhoto photo, CancellationToken cancellationToken = default)
    {
        await _context.VenuePhotos.AddAsync(photo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(VenuePhoto photo, CancellationToken cancellationToken = default)
    {
        _context.VenuePhotos.Remove(photo);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
