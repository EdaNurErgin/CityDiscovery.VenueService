using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using MediatR;


namespace CityDiscovery.Venues.Application.Features.Locations.Commands.AddDistrict;

public sealed class AddDistrictCommandHandler : IRequestHandler<AddDistrictCommand, int>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<AddDistrictCommandHandler> _logger;

    public AddDistrictCommandHandler(
        ILocationRepository locationRepository,
        ILogger<AddDistrictCommandHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<int> Handle(AddDistrictCommand request, CancellationToken cancellationToken)
    {
        // Üst şehir mevcut mu?
        var cityExists = await _locationRepository.CityExistsByIdAsync(request.CityId, cancellationToken);
        if (!cityExists)
            throw new KeyNotFoundException($"CityId={request.CityId} bulunamadı.");

        // Aynı şehirde aynı isimli semt var mı?
        var districtExists = await _locationRepository.DistrictExistsAsync(request.CityId, request.Name, cancellationToken);
        if (districtExists)
            throw new InvalidOperationException($"Bu şehirde '{request.Name}' adında bir semt zaten mevcut.");

        var district = District.Create(request.CityId, request.Name);
        await _locationRepository.AddDistrictAsync(district, cancellationToken);

        _logger.LogInformation("District '{Name}' added under CityId={CityId}, Id={Id}",
            district.Name, district.CityId, district.DistrictId);

        return district.DistrictId;
    }
}