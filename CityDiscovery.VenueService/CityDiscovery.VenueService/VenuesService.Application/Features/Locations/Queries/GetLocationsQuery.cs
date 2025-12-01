using CityDiscovery.Venues.Application.Features.Locations.Dtos;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Locations.Queries;

// 1. Ülkeleri Getir
public record GetCountriesQuery : IRequest<IReadOnlyList<CountryDto>>;

// 2. Şehirleri Getir
public record GetCitiesQuery(int CountryId) : IRequest<IReadOnlyList<CityDto>>;

// 3. İlçeleri Getir
public record GetDistrictsQuery(int CityId) : IRequest<IReadOnlyList<DistrictDto>>;

public sealed class LocationQueryHandler :
    IRequestHandler<GetCountriesQuery, IReadOnlyList<CountryDto>>,
    IRequestHandler<GetCitiesQuery, IReadOnlyList<CityDto>>,
    IRequestHandler<GetDistrictsQuery, IReadOnlyList<DistrictDto>>
{
    private readonly ILocationRepository _repository;

    public LocationQueryHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CountryDto>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var countries = await _repository.GetAllCountriesAsync(cancellationToken);
        return countries.Select(c => new CountryDto(c.CountryId, c.Name, c.Code)).ToList();
    }

    public async Task<IReadOnlyList<CityDto>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        var cities = await _repository.GetCitiesByCountryIdAsync(request.CountryId, cancellationToken);
        return cities.Select(c => new CityDto(c.CityId, c.Name)).ToList();
    }

    public async Task<IReadOnlyList<DistrictDto>> Handle(GetDistrictsQuery request, CancellationToken cancellationToken)
    {
        var districts = await _repository.GetDistrictsByCityIdAsync(request.CityId, cancellationToken);
        return districts.Select(d => new DistrictDto(d.DistrictId, d.Name)).ToList();
    }
}