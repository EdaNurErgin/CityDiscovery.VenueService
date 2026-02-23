using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using MediatR;


namespace CityDiscovery.Venues.Application.Features.Locations.Commands.AddCity;

public sealed class AddCityCommandHandler : IRequestHandler<AddCityCommand, int>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<AddCityCommandHandler> _logger;

    public AddCityCommandHandler(
        ILocationRepository locationRepository,
        ILogger<AddCityCommandHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<int> Handle(AddCityCommand request, CancellationToken cancellationToken)
    {
        // Üst ülke mevcut mu?
        var countryExists = await _locationRepository.CountryExistsByIdAsync(request.CountryId, cancellationToken);
        if (!countryExists)
            throw new KeyNotFoundException($"CountryId={request.CountryId} bulunamadı.");

        // Aynı ülkede aynı isimli şehir var mı?
        var cityExists = await _locationRepository.CityExistsAsync(request.CountryId, request.Name, cancellationToken);
        if (cityExists)
            throw new InvalidOperationException($"Bu ülkede '{request.Name}' adında bir şehir zaten mevcut.");

        var city = City.Create(request.CountryId, request.Name);
        await _locationRepository.AddCityAsync(city, cancellationToken);

        _logger.LogInformation("City '{Name}' added under CountryId={CountryId}, Id={Id}",
            city.Name, city.CountryId, city.CityId);

        return city.CityId;
    }
}