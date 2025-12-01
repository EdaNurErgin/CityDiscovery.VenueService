using CityDiscovery.Venues.Domain.Entities;

namespace CityDiscovery.Venues.Application.Interfaces.Repositories;

public interface ILocationRepository
{
    Task<IReadOnlyList<Country>> GetAllCountriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<City>> GetCitiesByCountryIdAsync(int countryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<District>> GetDistrictsByCityIdAsync(int cityId, CancellationToken cancellationToken = default);
}