using CityDiscovery.Venues.API.Models.Requests;
using CityDiscovery.Venues.Application.Features.Categories.Queries.GetAllCategories;
using CityDiscovery.Venues.Application.Features.Venues.Commands.ActivateVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.AddCategoryToVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.ApproveVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.CreateVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.DeactivateVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.RemoveCategoryFromVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateVenueBasicInfo;
using CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;
using CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenueCategories;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.API.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace CityDiscovery.Venues.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IVenueRepository _venueRepository;

    public VenuesController(IMediator mediator, IVenueRepository venueRepository)
    {
        _mediator = mediator;
        _venueRepository = venueRepository;
    }

    // POST /api/venues
    [HttpPost]
    public async Task<IActionResult> CreateVenue([FromBody] CreateVenueRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateVenueCommand(
            request.OwnerUserId,
            request.Name,
            request.Description,
            request.AddressText,
            request.Phone,
            request.WebsiteUrl,
            request.PriceLevel,
            request.OpeningHoursJson,
            request.Latitude,
            request.Longitude
        );

        var venueId = await _mediator.Send(command, cancellationToken);

        // 201 Created + Location header
        return CreatedAtAction(nameof(GetById), new { id = venueId }, new { id = venueId });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(id, cancellationToken);

        if (venue is null)
            return NotFound();

        var response = new
        {
            venue.Id,
            venue.Name,
            venue.Description,
            venue.AddressText,
            venue.Phone,
            venue.WebsiteUrl,
            PriceLevel = venue.PriceLevel?.Value,
            venue.OpeningHoursJson,
            venue.OwnerUserId,
            Location = new
            {
                Latitude = venue.Location.Y,   // NTS: Y = Latitude
                Longitude = venue.Location.X   // NTS: X = Longitude
            },
            venue.IsApproved,
            venue.IsActive,
            venue.CreatedAt
        };

        return Ok(response);
    }



    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var command = new ApproveVenueCommand(id);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    // PUT /api/venues/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateVenue(
        Guid id,
        [FromBody] UpdateVenueRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateVenueBasicInfoCommand(
            id,
            request.Name,
            request.Description,
            request.AddressText,
            request.Phone,
            request.WebsiteUrl,
            request.PriceLevel,
            request.OpeningHoursJson,
            request.Latitude,
            request.Longitude
        );

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    // PUT /api/venues/{id}/activate
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateVenueCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    // PUT /api/venues/{id:guid}/deactivate
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateVenueCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    // GET /api/venues/nearby?lat=..&lon=..&radius=..
    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double lat,
        [FromQuery] double lon,
        [FromQuery] double radius = 2000,
        CancellationToken cancellationToken = default)
    {
        var query = new GetNearbyVenuesQuery(lat, lon, radius);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Belirtilen venue'ya kategori ekler.
    /// </summary>
    [HttpPost("{venueId:guid}/categories")]
    public async Task<IActionResult> AddCategoryToVenue(
        Guid venueId,
        [FromBody] AddVenueCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCategoryToVenueCommand(venueId, request.CategoryId);

        await _mediator.Send(command, cancellationToken);

        // İçerik dönmediğimiz için 204 NoContent uygun
        return NoContent();
    }

    /// <summary>
    /// Belirtilen venue'dan kategoriyi kaldırır.
    /// </summary>
    [HttpDelete("{venueId:guid}/categories/{categoryId:int}")]
    public async Task<IActionResult> RemoveCategoryFromVenue(
        Guid venueId,
        int categoryId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveCategoryFromVenueCommand(venueId, categoryId);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Belirtilen venue'nun kategorilerini döner.
    /// </summary>
    [HttpGet("{venueId:guid}/categories")]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategoriesForVenue(
        Guid venueId,
        CancellationToken cancellationToken)
    {
        var query = new GetVenueCategoriesQuery(venueId);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }


}
