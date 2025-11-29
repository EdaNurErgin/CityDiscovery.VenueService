using CityDiscovery.Venues.Application.Features.Venues.Commands.AddPhotoToVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.RemovePhotoFromVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.UploadVenuePhoto;
using CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenuePhotos;
using CityDiscovery.Venues.API.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CityDiscovery.Venues.API.Controllers;

/// <summary>
/// Mekan fotoğraf yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/venues/{venueId:guid}/photos")]
[Authorize] // default: tüm endpoint'ler auth ister, aşağıda [AllowAnonymous] ile açacağız
[Tags("Venue Photos")]
public class VenuePhotosController : ControllerBase
{
    private readonly IMediator _mediator;

    public VenuePhotosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Mekanın fotoğraflarını listeler
    /// </summary>
    /// <param name="venueId">Mekan ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Mekan fotoğrafları listesi</returns>
    /// <response code="200">Başarılı - Fotoğraf listesi döner</response>
    /// <response code="404">Mekan bulunamadı</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<VenuePhotoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<VenuePhotoDto>>> GetPhotos(
        Guid venueId,
        CancellationToken cancellationToken)
    {
        var query = new GetVenuePhotosQuery(venueId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Mekana URL ile fotoğraf ekler
    /// </summary>
    /// <param name="venueId">Mekan ID</param>
    /// <param name="request">Fotoğraf bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Oluşturulan fotoğraf ID</returns>
    /// <response code="201">Başarılı - Fotoğraf eklendi</response>
    /// <response code="400">Geçersiz istek</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <remarks>
    /// Örnek istek:
    /// 
    /// POST /api/venues/{venueId}/photos
    /// {
    ///   "url": "https://example.com/photos/venue-photo.jpg",
    ///   "caption": "Mekanın dış görünümü",
    ///   "sortOrder": 1
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(Policy = "OwnerOnly")]
    [SwaggerOperation(
        Summary = "Mekana URL ile fotoğraf ekler",
        Description = "Owner rolündeki kullanıcılar mekana fotoğraf ekleyebilir.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddPhoto(
        Guid venueId,
        [FromBody] AddVenuePhotoRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddPhotoToVenueCommand(
            venueId,
            request.Url,
            request.Caption,
            request.SortOrder);

        var photoId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetPhotos),
            new { venueId },
            new { id = photoId });
    }

    /// <summary>
    /// Dosya upload ederek mekana fotoğraf ekler
    /// </summary>
    /// <param name="venueId">Mekan ID</param>
    /// <param name="file">Yüklenecek fotoğraf dosyası</param>
    /// <param name="caption">Fotoğraf açıklaması (opsiyonel)</param>
    /// <param name="sortOrder">Sıralama (opsiyonel)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Oluşturulan fotoğraf ID</returns>
    /// <response code="201">Başarılı - Fotoğraf yüklendi</response>
    /// <response code="400">Geçersiz dosya veya istek</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    [HttpPost("upload")]
    [Authorize(Policy = "OwnerOnly")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UploadPhoto(
        Guid venueId,
        IFormFile file,
        [FromForm] string? caption,
        [FromForm] int? sortOrder,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        var command = new UploadVenuePhotoCommand(
            venueId,
            file.OpenReadStream(),
            file.FileName,
            file.ContentType ?? "application/octet-stream",
            caption,
            sortOrder);

        var photoId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetPhotos),
            new { venueId },
            new { id = photoId });
    }

    /// <summary>
    /// Mekandan fotoğraf siler
    /// </summary>
    /// <param name="venueId">Mekan ID</param>
    /// <param name="photoId">Silinecek fotoğraf ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Fotoğraf silindi</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <response code="404">Fotoğraf bulunamadı</response>
    [HttpDelete("{photoId:guid}")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemovePhoto(
        Guid venueId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var command = new RemovePhotoFromVenueCommand(venueId, photoId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}

