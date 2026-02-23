using CityDiscovery.Venues.Domain.Entities;

namespace CityDiscovery.Venues.Application.Interfaces.Repositories;

public interface ILocationRepository
{
    Task<IReadOnlyList<Country>> GetAllCountriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<City>> GetCitiesByCountryIdAsync(int countryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<District>> GetDistrictsByCityIdAsync(int cityId, CancellationToken cancellationToken = default);

    // ─── Admin: Ülke ────────────────────────────────────────────────────────
    Task<bool> CountryExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> CountryExistsByIdAsync(int countryId, CancellationToken cancellationToken = default);
    Task AddCountryAsync(Country country, CancellationToken cancellationToken = default);
    Task DeleteCountryAsync(int countryId, CancellationToken cancellationToken = default);

    // ─── Admin: Şehir ───────────────────────────────────────────────────────
    Task<bool> CityExistsAsync(int countryId, string name, CancellationToken cancellationToken = default);
    Task<bool> CityExistsByIdAsync(int cityId, CancellationToken cancellationToken = default);
    Task AddCityAsync(City city, CancellationToken cancellationToken = default);
    Task DeleteCityAsync(int cityId, CancellationToken cancellationToken = default);

    // ─── Admin: Semt ────────────────────────────────────────────────────────
    Task<bool> DistrictExistsAsync(int cityId, string name, CancellationToken cancellationToken = default);
    Task<bool> DistrictExistsByIdAsync(int districtId, CancellationToken cancellationToken = default);
    Task AddDistrictAsync(District district, CancellationToken cancellationToken = default);
    Task DeleteDistrictAsync(int districtId, CancellationToken cancellationToken = default);
}