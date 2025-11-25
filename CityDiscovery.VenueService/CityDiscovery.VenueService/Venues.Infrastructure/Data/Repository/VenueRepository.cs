using CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Types;
using NetTopologySuite;
using NetTopologySuite.Geometries;


namespace CityDiscovery.Venues.Infrastructure.Persistence.Repository;

public sealed class VenueRepository : IVenueRepository
{
    private readonly VenueDbContext _context;

    public VenueRepository(VenueDbContext context)
    {
        _context = context;
    }

    //public async Task<Venuex?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    //{
    //    return await _context.Venues
    //        .Include(v => v.Address)
    //        .Include(v => v.VenueCategories)
    //        .Include(v => v.Photos)
    //        .Include(v => v.MenuCategories)
    //            .ThenInclude(mc => mc.Items)
    //        .Include(v => v.Events)
    //        .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    //}
    public async Task<Venuex?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Venues
            .Include(v => v.Address)
            .Include(v => v.VenueCategories)      // 🔹 Join tablosu
                .ThenInclude(vc => vc.Category)   // 🔹 Category navigation’ı da yüklensin
            .Include(v => v.Photos)
            .Include(v => v.MenuCategories)
                .ThenInclude(mc => mc.Items)
            .Include(v => v.Events)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Venuex?> GetByOwnerIdAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Venues
            .FirstOrDefaultAsync(v => v.OwnerUserId == ownerUserId, cancellationToken);
    }

    public async Task<bool> OwnerHasVenueAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Venues
            .AnyAsync(v => v.OwnerUserId == ownerUserId, cancellationToken);
    }

    public async Task AddAsync(Venuex venue, CancellationToken cancellationToken = default)
    {
        await _context.Venues.AddAsync(venue, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Venuex venue, CancellationToken cancellationToken = default)
    {
        _context.Venues.Update(venue);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<List<NearbyVenueDto>> GetNearbyVenuesAsync(
     double latitude,
     double longitude,
     double radiusInMeters,
     CancellationToken cancellationToken = default)
    {
        // 1) EF → SQL: Sadece data çekiyoruz
        var venues = await _context.Venues
            .Where(v => v.IsApproved && v.IsActive)
            .ToListAsync(cancellationToken);

        // 2) NTS ile distance hesabı (bellekte)
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        // Not: Point(X=lon, Y=lat)
        var originPoint = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));

        var result = venues
            .Select(v =>
            {
                // v.Location zaten Point (snapshot öyle gösteriyor)
                var venuePoint = v.Location;

                // Mesafe (metre cinsinden, SRID 4326 ile uyumlu)
                var distance = originPoint.Distance(venuePoint);

                return new
                {
                    v.Id,
                    v.Name,
                    Lat = venuePoint.Y, // Y = Latitude
                    Lon = venuePoint.X, // X = Longitude
                    Distance = distance
                };
            })
            .Where(x => x.Distance <= radiusInMeters)
            .OrderBy(x => x.Distance)
            .Select(x => new NearbyVenueDto(
                x.Id,
                x.Name,
                x.Lat,
                x.Lon,
                x.Distance
            ))
            .ToList();

        return result;
    }


}
