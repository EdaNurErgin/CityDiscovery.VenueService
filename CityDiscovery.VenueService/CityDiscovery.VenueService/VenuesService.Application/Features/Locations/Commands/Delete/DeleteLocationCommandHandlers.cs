using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;


namespace CityDiscovery.Venues.Application.Features.Locations.Commands.Delete;



public sealed class DeleteLocationCommandHandlers : IRequestHandler<DeleteCountryCommand, Unit>
{
    private readonly ILocationRepository _repo;
    private readonly ILogger<DeleteLocationCommandHandlers> _logger;

    public DeleteLocationCommandHandlers(ILocationRepository repo, ILogger<DeleteLocationCommandHandlers> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repo.CountryExistsByIdAsync(request.CountryId, cancellationToken);
        if (!exists)
            throw new KeyNotFoundException($"CountryId={request.CountryId} bulunamadı.");

        await _repo.DeleteCountryAsync(request.CountryId, cancellationToken);

        _logger.LogInformation("Country Id={Id} deleted", request.CountryId);
        return Unit.Value;
    }
}



public sealed class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, Unit>
{
    private readonly ILocationRepository _repo;
    private readonly ILogger<DeleteCityCommandHandler> _logger;

    public DeleteCityCommandHandler(ILocationRepository repo, ILogger<DeleteCityCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repo.CityExistsByIdAsync(request.CityId, cancellationToken);
        if (!exists)
            throw new KeyNotFoundException($"CityId={request.CityId} bulunamadı.");

        await _repo.DeleteCityAsync(request.CityId, cancellationToken);

        _logger.LogInformation("City Id={Id} deleted", request.CityId);
        return Unit.Value;
    }
}



public sealed class DeleteDistrictCommandHandler : IRequestHandler<DeleteDistrictCommand, Unit>
{
    private readonly ILocationRepository _repo;
    private readonly ILogger<DeleteDistrictCommandHandler> _logger;

    public DeleteDistrictCommandHandler(ILocationRepository repo, ILogger<DeleteDistrictCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteDistrictCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repo.DistrictExistsByIdAsync(request.DistrictId, cancellationToken);
        if (!exists)
            throw new KeyNotFoundException($"DistrictId={request.DistrictId} bulunamadı.");

        await _repo.DeleteDistrictAsync(request.DistrictId, cancellationToken);

        _logger.LogInformation("District Id={Id} deleted", request.DistrictId);
        return Unit.Value;
    }
}