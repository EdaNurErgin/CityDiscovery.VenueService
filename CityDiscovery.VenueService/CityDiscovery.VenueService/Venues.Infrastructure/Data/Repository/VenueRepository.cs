using CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Types;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Globalization;
using System.Text.Json;
using CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;


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

    public async Task DeleteAsync(Venuex venue, CancellationToken cancellationToken = default)
    {
        _context.Venues.Remove(venue);
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


    public async Task<List<NearbyVenueDto>> SearchVenuesAsync(
    double latitude,
    double longitude,
    double radiusInMeters,
    int? categoryId,
    byte? minPriceLevel,
    byte? maxPriceLevel,
    bool? openNow,
    CancellationToken cancellationToken = default)
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var originPoint = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));

        // 1) DB'den temel listeyi çek (onaylı + aktif)
        var query = _context.Venues
            .Include(v => v.VenueCategories) // kategori filtresi için
            //.Where(v => v.IsApproved && v.IsActive)
            .Where(v => v.IsActive)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(v =>
                v.VenueCategories.Any(vc => vc.CategoryId == categoryId.Value));
        }

        // Hepsini belleğe alıyoruz (PriceLevel & OpeningHours için C# tarafında filtreleyeceğiz)
        var venues = await query.ToListAsync(cancellationToken);

        // 2) PriceLevel filtresi (C# tarafında)
        if (minPriceLevel.HasValue)
        {
            venues = venues
                .Where(v => v.PriceLevel != null &&
                            v.PriceLevel.Value >= minPriceLevel.Value)
                .ToList();
        }

        if (maxPriceLevel.HasValue)
        {
            venues = venues
                .Where(v => v.PriceLevel != null &&
                            v.PriceLevel.Value <= maxPriceLevel.Value)
                .ToList();
        }

        // 3) "Şu an açık mı?" filtresi
        if (openNow == true)
        {
            var now = DateTime.UtcNow; // istersen TimeZone ekleyebiliriz ileride
            venues = venues
                .Where(v => IsOpenNow(v.OpeningHoursJson, now))
                .ToList();
        }

        // 4) Mesafe hesabı ve radius filtresi
        var result = venues
            .Select(v =>
            {
                var venuePoint = v.Location;
                var distance = originPoint.Distance(venuePoint);

                return new
                {
                    v.Id,
                    v.Name,
                    Lat = venuePoint.Y,
                    Lon = venuePoint.X,
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
                x.Distance))
            .ToList();

        return result;
    }

    
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Venues
            .AnyAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<Venuex>> GetByIdsAsync(List<Guid> venueIds, CancellationToken cancellationToken = default)
    {
        return await _context.Venues
            .Where(v => venueIds.Contains(v.Id))
            .ToListAsync(cancellationToken);
    }

    private static bool IsOpenNow(string? openingHoursJson, DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(openingHoursJson))
            return false;

        try
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(openingHoursJson);
            if (dict is null)
                return false;

            // Örn: {"Mon":"09:00-22:00"}
            var dayKey = nowUtc.ToString("ddd", CultureInfo.InvariantCulture); // Mon, Tue...

            if (!dict.TryGetValue(dayKey, out var range))
                return false;

            var parts = range.Split('-', 2);
            if (parts.Length != 2)
                return false;

            if (!TimeSpan.TryParse(parts[0], out var openTime))
                return false;

            if (!TimeSpan.TryParse(parts[1], out var closeTime))
                return false;

            var nowTime = nowUtc.TimeOfDay;
            return nowTime >= openTime && nowTime <= closeTime;
        }
        catch
        {
            // JSON bozuksa "kapalı" kabul et
            return false;
        }
    }




}
