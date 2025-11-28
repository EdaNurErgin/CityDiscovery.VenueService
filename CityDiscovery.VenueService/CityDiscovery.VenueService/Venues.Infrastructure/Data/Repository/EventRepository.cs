using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Repository;

public sealed class EventRepository : IEventRepository
{
    private readonly VenueDbContext _context;

    public EventRepository(VenueDbContext context)
    {
        _context = context;
    }

    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetActiveEventsForVenueAsync(
        Guid venueId,
        DateTime? nowUtc = null,
        CancellationToken cancellationToken = default)
    {
        var now = nowUtc ?? DateTime.UtcNow;

        return await _context.Events
            .Where(e =>
                e.VenueId == venueId &&
                e.IsActive &&
                e.StartDate <= now &&
                (e.EndDate == null || e.EndDate >= now))
            .OrderBy(e => e.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await _context.Events.AddAsync(@event, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Event @event, CancellationToken cancellationToken = default)
    {
        _context.Events.Update(@event);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Event @event, CancellationToken cancellationToken = default)
    {
        _context.Events.Remove(@event);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetEventsForVenueAsync(
    Guid venueId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Where(e => e.VenueId == venueId)  // istersen && e.IsActive ekle
            .OrderBy(e => e.StartDate)
            .ToListAsync(cancellationToken);
    }
}
