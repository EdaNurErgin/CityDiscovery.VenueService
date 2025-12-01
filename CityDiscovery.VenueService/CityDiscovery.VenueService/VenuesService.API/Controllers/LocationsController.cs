using CityDiscovery.Venues.Application.Features.Locations.Dtos;
using CityDiscovery.Venues.Application.Features.Locations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityDiscovery.Venues.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tüm ülkeleri listeler
    /// </summary>
    [HttpGet("countries")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<CountryDto>>> GetCountries()
    {
        var result = await _mediator.Send(new GetCountriesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Seçilen ülkeye ait şehirleri listeler
    /// </summary>
    [HttpGet("countries/{countryId}/cities")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<CityDto>>> GetCities(int countryId)
    {
        var result = await _mediator.Send(new GetCitiesQuery(countryId));
        return Ok(result);
    }

    /// <summary>
    /// Seçilen şehre ait ilçeleri listeler
    /// </summary>
    [HttpGet("cities/{cityId}/districts")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<DistrictDto>>> GetDistricts(int cityId)
    {
        var result = await _mediator.Send(new GetDistrictsQuery(cityId));
        return Ok(result);
    }
}