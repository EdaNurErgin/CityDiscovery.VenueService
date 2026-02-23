using CityDiscovery.Venues.Application.Features.Locations.Commands.AddCity;
using CityDiscovery.Venues.Application.Features.Locations.Commands.AddCountry;
using CityDiscovery.Venues.Application.Features.Locations.Commands.AddDistrict;
using CityDiscovery.Venues.Application.Features.Locations.Commands.Delete;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CityDiscovery.VenueService.VenuesService.API.Models.Requests;

namespace CityDiscovery.Venues.API.Controllers;

/// <summary>
/// Admin - Ülke / Şehir / Semt yönetimi.
/// Tüm endpoint'ler Admin rolü gerektirir.
/// </summary>
[ApiController]
[Route("api/admin/locations")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public sealed class AdminLocationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILocationRepository _locationRepository;

    public AdminLocationController(IMediator mediator, ILocationRepository locationRepository)
    {
        _mediator = mediator;
        _locationRepository = locationRepository;
    }

 

    /// <summary>Tüm ülkeleri listeler.</summary>
    [HttpGet("countries")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCountries(CancellationToken cancellationToken)
    {
        var countries = await _locationRepository.GetAllCountriesAsync(cancellationToken);
        return Ok(countries);
    }

    /// <summary>Yeni ülke ekler.</summary>
    [HttpPost("countries")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddCountry(
        [FromBody] AddCountryRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new AddCountryCommand(request.Name, request.Code), cancellationToken);
        return CreatedAtAction(nameof(GetCountries), new { }, new { id });
    }

    /// <summary>Ülkeyi siler. Cascade silinecek şehirler/semtler DB kısıtına tabidir.</summary>
    [HttpDelete("countries/{countryId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCountry(int countryId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCountryCommand(countryId), cancellationToken);
        return NoContent();
    }


    /// <summary>Belirli ülkedeki şehirleri listeler.</summary>
    [HttpGet("countries/{countryId:int}/cities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCities(int countryId, CancellationToken cancellationToken)
    {
        var cities = await _locationRepository.GetCitiesByCountryIdAsync(countryId, cancellationToken);
        return Ok(cities);
    }

    /// <summary>Belirli ülkeye yeni şehir ekler.</summary>
    [HttpPost("countries/{countryId:int}/cities")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddCity(
        int countryId,
        [FromBody] AddCityRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new AddCityCommand(countryId, request.Name), cancellationToken);
        return CreatedAtAction(nameof(GetCities), new { countryId }, new { id });
    }

    /// <summary>Şehri siler.</summary>
    [HttpDelete("cities/{cityId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCity(int cityId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCityCommand(cityId), cancellationToken);
        return NoContent();
    }

   

    /// <summary>Belirli şehirdeki semtleri listeler.</summary>
    [HttpGet("cities/{cityId:int}/districts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDistricts(int cityId, CancellationToken cancellationToken)
    {
        var districts = await _locationRepository.GetDistrictsByCityIdAsync(cityId, cancellationToken);
        return Ok(districts);
    }

    /// <summary>Belirli şehire yeni semt ekler.</summary>
    [HttpPost("cities/{cityId:int}/districts")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddDistrict(
        int cityId,
        [FromBody] AddDistrictRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new AddDistrictCommand(cityId, request.Name), cancellationToken);
        return CreatedAtAction(nameof(GetDistricts), new { cityId }, new { id });
    }

    /// <summary>Semti siler.</summary>
    [HttpDelete("districts/{districtId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDistrict(int districtId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteDistrictCommand(districtId), cancellationToken);
        return NoContent();
    }
}

// ─── Request DTO'ları ──────────────────────────────────────────────────────



