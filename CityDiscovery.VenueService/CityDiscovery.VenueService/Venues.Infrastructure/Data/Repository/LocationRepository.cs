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

    // ─── Admin: Ülke ─────────────────────────────────────────────────────────

    public Task<bool> CountryExistsAsync(string name, CancellationToken cancellationToken = default)
        => _context.Countries.AnyAsync(c => c.Name == name, cancellationToken);

    public Task<bool> CountryExistsByIdAsync(int countryId, CancellationToken cancellationToken = default)
        => _context.Countries.AnyAsync(c => c.CountryId == countryId, cancellationToken);

    public async Task AddCountryAsync(Country country, CancellationToken cancellationToken = default)
    {
        _context.Countries.Add(country);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCountryAsync(int countryId, CancellationToken cancellationToken = default)
    {
        var country = await _context.Countries.FindAsync([countryId], cancellationToken)
            ?? throw new KeyNotFoundException($"CountryId={countryId} bulunamadı.");

        _context.Countries.Remove(country);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // ─── Admin: Şehir ────────────────────────────────────────────────────────

    public Task<bool> CityExistsAsync(int countryId, string name, CancellationToken cancellationToken = default)
        => _context.Cities.AnyAsync(c => c.CountryId == countryId && c.Name == name, cancellationToken);

    public Task<bool> CityExistsByIdAsync(int cityId, CancellationToken cancellationToken = default)
        => _context.Cities.AnyAsync(c => c.CityId == cityId, cancellationToken);

    public async Task AddCityAsync(City city, CancellationToken cancellationToken = default)
    {
        _context.Cities.Add(city);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCityAsync(int cityId, CancellationToken cancellationToken = default)
    {
        var city = await _context.Cities.FindAsync([cityId], cancellationToken)
            ?? throw new KeyNotFoundException($"CityId={cityId} bulunamadı.");

        _context.Cities.Remove(city);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // ─── Admin: Semt ─────────────────────────────────────────────────────────

    public Task<bool> DistrictExistsAsync(int cityId, string name, CancellationToken cancellationToken = default)
        => _context.Districts.AnyAsync(d => d.CityId == cityId && d.Name == name, cancellationToken);

    public Task<bool> DistrictExistsByIdAsync(int districtId, CancellationToken cancellationToken = default)
        => _context.Districts.AnyAsync(d => d.DistrictId == districtId, cancellationToken);

    public async Task AddDistrictAsync(District district, CancellationToken cancellationToken = default)
    {
        _context.Districts.Add(district);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteDistrictAsync(int districtId, CancellationToken cancellationToken = default)
    {
        var district = await _context.Districts.FindAsync([districtId], cancellationToken)
            ?? throw new KeyNotFoundException($"DistrictId={districtId} bulunamadı.");

        _context.Districts.Remove(district);
        await _context.SaveChangesAsync(cancellationToken);
    }
}