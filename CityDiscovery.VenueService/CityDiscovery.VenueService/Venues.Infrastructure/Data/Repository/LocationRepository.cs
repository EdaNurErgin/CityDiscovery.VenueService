using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Repositories;

public sealed class LocationRepository : ILocationRepository
{
    private readonly VenueDbContext _context;

    public LocationRepository(VenueDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Country>> GetAllCountriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Countries
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<City>> GetCitiesByCountryIdAsync(int countryId, CancellationToken cancellationToken = default)
    {
        return await _context.Cities
            .Where(c => c.CountryId == countryId)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<District>> GetDistrictsByCityIdAsync(int cityId, CancellationToken cancellationToken = default)
    {
        return await _context.Districts
            .Where(d => d.CityId == cityId)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }
}