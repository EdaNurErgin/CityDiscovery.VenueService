using CityDiscovery.Venues.Application.Features.Venues.Commands.CreateEvent;
using CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteEvent;
using CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateEvent;
using CityDiscovery.Venues.Application.Features.Venues.Commands.UploadEventImage;
using CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenueEvents;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Application.Interfaces.Services;
using CityDiscovery.VenueService.VenuesService.API.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CityDiscovery.Venues.API.Controllers;

/// <summary>
/// Mekan etkinlik yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/venues")]
[Authorize]
[Tags("Events")]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IVenueRepository _venueRepository;
    private readonly ICurrentUserService _currentUser;

    public EventsController(
        IMediator mediator,
        IVenueRepository venueRepository,
        ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _venueRepository = venueRepository;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Mekanın aktif etkinliklerini listeler
    /// </summary>
    /// <param name="venueId">Mekan ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Etkinlik listesi</returns>
    /// <response code="200">Başarılı - Etkinlik listesi döner</response>
    /// <response code="404">Mekan bulunamadı</response>
    [HttpGet("{venueId:guid}/events")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetEvents(
        Guid venueId,
        CancellationToken cancellationToken)
    {
        var query = new GetVenueEventsQuery(venueId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Giriş yapmış kullanıcının mekanına ait aktif etkinlikleri listeler
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Etkinlik listesi</returns>
    /// <response code="200">Başarılı - Etkinlik listesi döner</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="404">Mekan bulunamadı</response>
    [HttpGet("my/events")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(typeof(IReadOnlyList<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetMyEvents(
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        var venue = await _venueRepository.GetByOwnerIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (venue is null)
            return NotFound("Bu kullanıcıya ait bir mekan bulunamadı.");

        var query = new GetVenueEventsQuery(venue.Id);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Mekana yeni etkinlik ekler
    /// </summary>
    /// <param name="venueId">Mekan ID</param>
    /// <param name="request">Etkinlik bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Oluşturulan etkinlik ID</returns>
    /// <response code="201">Başarılı - Etkinlik oluşturuldu</response>
    /// <response code="400">Geçersiz istek</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <remarks>
    /// Örnek istek:
    /// 
    /// POST /api/venues/{venueId}/events
    /// {
    ///   "title": "Canlı Müzik Gecesi",
    ///   "description": "Her Cuma akşamı canlı müzik",
    ///   "startDate": "2025-12-01T20:00:00Z",
    ///   "endDate": "2025-12-01T23:00:00Z",
    ///   "imageUrl": "https://example.com/photos/live-music.jpg"
    /// }
    /// 
    /// Tarih formatı: ISO 8601 (UTC zaman dilimi önerilir)
    /// </remarks>
    [HttpPost("{venueId:guid}/events")]
    [Authorize(Policy = "OwnerOnly")]
    [SwaggerOperation(
        Summary = "Mekana yeni etkinlik ekler",
        Description = "Owner rolündeki kullanıcılar mekana etkinlik ekleyebilir.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateEvent(
        Guid venueId,
        [FromBody] CreateEventRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateEventCommand(
            venueId,
            request.Title,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.ImageUrl);

        var eventId = await _mediator.Send(command, cancellationToken);
        return Created("", new { id = eventId });
    }

    /// <summary>
    /// Etkinliği günceller
    /// </summary>
    /// <param name="eventId">Etkinlik ID</param>
    /// <param name="request">Güncellenecek bilgiler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Etkinlik güncellendi</response>
    /// <response code="400">Geçersiz istek</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <response code="404">Etkinlik bulunamadı</response>
    [HttpPut("events/{eventId:guid}")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEvent(
        Guid eventId,
        [FromBody] UpdateEventRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEventCommand(
            eventId,
            request.Title,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.ImageUrl,
            request.IsActive);

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Etkinliği siler
    /// </summary>
    /// <param name="eventId">Etkinlik ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Etkinlik silindi</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <response code="404">Etkinlik bulunamadı</response>
    [HttpDelete("events/{eventId:guid}")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEvent(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteEventCommand(eventId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Etkinliğe görsel yükler
    /// </summary>
    /// <param name="eventId">Etkinlik ID</param>
    /// <param name="file">Yüklenecek görsel dosyası</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Görsel yüklendi</response>
    /// <response code="400">Geçersiz dosya</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    [HttpPost("events/{eventId:guid}/image-upload")]
    [Authorize(Policy = "OwnerOnly")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UploadEventImage(
        Guid eventId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        var command = new UploadEventImageCommand(
            eventId,
            file.OpenReadStream(),
            file.FileName,
            file.ContentType ?? "application/octet-stream"
        );

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}

