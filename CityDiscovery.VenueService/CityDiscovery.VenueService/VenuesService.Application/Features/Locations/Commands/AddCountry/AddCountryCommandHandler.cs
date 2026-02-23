using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using MediatR;


namespace CityDiscovery.Venues.Application.Features.Locations.Commands.AddCountry;

public sealed class AddCountryCommandHandler : IRequestHandler<AddCountryCommand, int>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<AddCountryCommandHandler> _logger;

    public AddCountryCommandHandler(
        ILocationRepository locationRepository,
        ILogger<AddCountryCommandHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<int> Handle(AddCountryCommand request, CancellationToken cancellationToken)
    {
        // Aynı isimde ülke var mı?
        var exists = await _locationRepository.CountryExistsAsync(request.Name, cancellationToken);
        if (exists)
            throw new InvalidOperationException($"'{request.Name}' adında bir ülke zaten mevcut.");

        var country = Country.Create(request.Name, request.Code);
        await _locationRepository.AddCountryAsync(country, cancellationToken);

        _logger.LogInformation("Country '{Name}' added with Id={Id}", country.Name, country.CountryId);

        return country.CountryId;
    }
}