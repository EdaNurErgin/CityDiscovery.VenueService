//using CityDiscovery.Venues.API.Models.Requests;
//using CityDiscovery.Venues.Application.Features.Categories.Queries.GetAllCategories;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.ActivateVenue;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.AddCategoryToVenue;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.AddPhotoToVenue;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.ApproveVenue;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.CreateMenuCategory;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.CreateMenuItem;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.CreateVenue;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.DeactivateVenue;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteMenuCategory;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteMenuItem;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.RemoveCategoryFromVenue;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.RemovePhotoFromVenue;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateMenuCategory;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateMenuItem;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateVenueBasicInfo;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.UploadMenuItemPhoto;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.UploadVenuePhoto;
//using CityDiscovery.Venues.Application.Features.Venues.Queries.GetMenuForVenue;
//using CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;
//using CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenueCategories;
//using CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenuePhotos;
//using CityDiscovery.Venues.Application.Interfaces.Repositories;
//using CityDiscovery.VenueService.VenuesService.API.Models.Requests;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.CreateEvent;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateEvent;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteEvent;
//using CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenueEvents;
//using CityDiscovery.Venues.Application.Features.Venues.Commands.UploadEventImage;
//using CityDiscovery.Venues.Application.Features.Venues.Queries.SearchVenues;
//using CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;
//using System.Security.Claims;
//using CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenueEvents;







//namespace CityDiscovery.Venues.API.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class VenuesController : ControllerBase
//{
//    private readonly IMediator _mediator;
//    private readonly IVenueRepository _venueRepository;

//    public VenuesController(IMediator mediator, IVenueRepository venueRepository)
//    {
//        _mediator = mediator;
//        _venueRepository = venueRepository;
//    }


//    private Guid GetCurrentUserId()
//    {
//        // Token içindeki user id claim’ini bulmaya çalışıyoruz
//        var userIdClaim =
//            User.FindFirst("sub") ??
//            User.FindFirst("userId") ??
//            User.FindFirst(ClaimTypes.NameIdentifier);

//        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
//        {

//            throw new UnauthorizedAccessException("User id not found in token.");
//        }

//        return userId;


//    }

//    /// <summary>
//    /// Giriş yapmış kullanıcının sahip olduğu mekanı döner.
//    /// </summary>
//    [HttpGet("my")]
//    public async Task<IActionResult> GetMyVenue(CancellationToken cancellationToken)
//    {
//        var ownerUserId = GetCurrentUserId();

//        var venue = await _venueRepository.GetByOwnerIdAsync(ownerUserId, cancellationToken);

//        if (venue is null)
//            return NotFound("Bu kullanıcıya ait bir mekan bulunamadı.");

//        var response = new
//        {
//            venue.Id,
//            venue.Name,
//            venue.Description,
//            venue.AddressText,
//            venue.Phone,
//            venue.WebsiteUrl,
//            PriceLevel = venue.PriceLevel?.Value,
//            venue.OpeningHoursJson,
//            venue.OwnerUserId,
//            Location = new
//            {
//                Latitude = venue.Location.Y,
//                Longitude = venue.Location.X
//            },
//            venue.IsApproved,
//            venue.IsActive,
//            venue.CreatedAt
//        };

//        return Ok(response);
//    }


//    /// <summary>
//    /// Giriş yapmış kullanıcının mekanının menüsünü döner.
//    /// </summary>
//    [HttpGet("my/menu")]
//    public async Task<ActionResult<IReadOnlyList<MenuCategoryDto>>> GetMyMenu(
//        CancellationToken cancellationToken)
//    {
//        var ownerUserId = GetCurrentUserId();

//        var venue = await _venueRepository.GetByOwnerIdAsync(ownerUserId, cancellationToken);
//        if (venue is null)
//            return NotFound("Bu kullanıcıya ait bir mekan bulunamadı.");

//        var query = new GetMenuForVenueQuery(venue.Id);

//        var result = await _mediator.Send(query, cancellationToken);

//        return Ok(result);
//    }




//    // POST /api/venues
//    [HttpPost]
//    public async Task<IActionResult> CreateVenue([FromBody] CreateVenueRequest request, CancellationToken cancellationToken)
//    {
//        var command = new CreateVenueCommand(
//            request.OwnerUserId,
//            request.Name,
//            request.Description,
//            request.AddressText,
//            request.Phone,
//            request.WebsiteUrl,
//            request.PriceLevel,
//            request.OpeningHoursJson,
//            request.Latitude,
//            request.Longitude
//        );

//        var venueId = await _mediator.Send(command, cancellationToken);

//        // 201 Created + Location header
//        return CreatedAtAction(nameof(GetById), new { id = venueId }, new { id = venueId });
//    }

//    [HttpGet("{id:guid}")]
//    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
//    {
//        var venue = await _venueRepository.GetByIdAsync(id, cancellationToken);

//        if (venue is null)
//            return NotFound();

//        var response = new
//        {
//            venue.Id,
//            venue.Name,
//            venue.Description,
//            venue.AddressText,
//            venue.Phone,
//            venue.WebsiteUrl,
//            PriceLevel = venue.PriceLevel?.Value,
//            venue.OpeningHoursJson,
//            venue.OwnerUserId,
//            Location = new
//            {
//                Latitude = venue.Location.Y,   // NTS: Y = Latitude
//                Longitude = venue.Location.X   // NTS: X = Longitude
//            },
//            venue.IsApproved,
//            venue.IsActive,
//            venue.CreatedAt
//        };

//        return Ok(response);
//    }



//    [HttpPut("{id:guid}/approve")]
//    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
//    {
//        var command = new ApproveVenueCommand(id);

//        await _mediator.Send(command, cancellationToken);

//        return NoContent();
//    }

//    // PUT /api/venues/{id}
//    [HttpPut("{id:guid}")]
//    public async Task<IActionResult> UpdateVenue(
//        Guid id,
//        [FromBody] UpdateVenueRequest request,
//        CancellationToken cancellationToken)
//    {
//        var command = new UpdateVenueBasicInfoCommand(
//            id,
//            request.Name,
//            request.Description,
//            request.AddressText,
//            request.Phone,
//            request.WebsiteUrl,
//            request.PriceLevel,
//            request.OpeningHoursJson,
//            request.Latitude,
//            request.Longitude
//        );

//        await _mediator.Send(command, cancellationToken);

//        return NoContent();
//    }

//    // PUT /api/venues/{id}/activate
//    [HttpPut("{id:guid}/activate")]
//    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
//    {
//        var command = new ActivateVenueCommand(id);
//        await _mediator.Send(command, cancellationToken);
//        return NoContent();
//    }

//    // PUT /api/venues/{id:guid}/deactivate
//    [HttpPut("{id:guid}/deactivate")]
//    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
//    {
//        var command = new DeactivateVenueCommand(id);
//        await _mediator.Send(command, cancellationToken);
//        return NoContent();
//    }

//    // GET /api/venues/nearby?lat=..&lon=..&radius=..
//    [HttpGet("nearby")]
//    public async Task<IActionResult> GetNearby(
//        [FromQuery] double lat,
//        [FromQuery] double lon,
//        [FromQuery] double radius = 2000,
//        CancellationToken cancellationToken = default)
//    {
//        var query = new GetNearbyVenuesQuery(lat, lon, radius);

//        var result = await _mediator.Send(query, cancellationToken);

//        return Ok(result);
//    }

//    /// <summary>
//    /// Belirtilen venue'ya kategori ekler.
//    /// </summary>
//    [HttpPost("{venueId:guid}/categories")]
//    public async Task<IActionResult> AddCategoryToVenue(
//        Guid venueId,
//        [FromBody] AddVenueCategoryRequest request,
//        CancellationToken cancellationToken)
//    {
//        var command = new AddCategoryToVenueCommand(venueId, request.CategoryId);

//        await _mediator.Send(command, cancellationToken);

//        // İçerik dönmediğimiz için 204 NoContent uygun
//        return NoContent();
//    }

//    /// <summary>
//    /// Belirtilen venue'dan kategoriyi kaldırır.
//    /// </summary>
//    [HttpDelete("{venueId:guid}/categories/{categoryId:int}")]
//    public async Task<IActionResult> RemoveCategoryFromVenue(
//        Guid venueId,
//        int categoryId,
//        CancellationToken cancellationToken)
//    {
//        var command = new RemoveCategoryFromVenueCommand(venueId, categoryId);

//        await _mediator.Send(command, cancellationToken);

//        return NoContent();
//    }

//    /// <summary>
//    /// Belirtilen venue'nun kategorilerini döner.
//    /// </summary>
//    [HttpGet("{venueId:guid}/categories")]
//    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategoriesForVenue(
//        Guid venueId,
//        CancellationToken cancellationToken)
//    {
//        var query = new GetVenueCategoriesQuery(venueId);

//        var result = await _mediator.Send(query, cancellationToken);

//        return Ok(result);
//    }


//    /// <summary>
//    /// Mekanın fotoğraflarını döner.
//    /// </summary>
//    [HttpGet("{venueId:guid}/photos")]
//    public async Task<ActionResult<IReadOnlyList<VenuePhotoDto>>> GetPhotosForVenue(
//        Guid venueId,
//        CancellationToken cancellationToken)
//    {
//        var query = new GetVenuePhotosQuery(venueId);
//        var result = await _mediator.Send(query, cancellationToken);
//        return Ok(result);
//    }

//    /// <summary>
//    /// Mekana yeni fotoğraf ekler.
//    /// </summary>
//    [HttpPost("{venueId:guid}/photos")]
//    public async Task<IActionResult> AddPhotoToVenue(
//        Guid venueId,
//        [FromBody] AddVenuePhotoRequest request,
//        CancellationToken cancellationToken)
//    {
//        var command = new AddPhotoToVenueCommand(
//            venueId,
//            request.Url,
//            request.Caption,
//            request.SortOrder);

//        var photoId = await _mediator.Send(command, cancellationToken);

//        return CreatedAtAction(nameof(GetPhotosForVenue),
//            new { venueId },
//            new { id = photoId });
//    }

//    /// <summary>
//    /// Mekandan fotoğraf siler.
//    /// </summary>
//    [HttpDelete("{venueId:guid}/photos/{photoId:guid}")]
//    public async Task<IActionResult> RemovePhotoFromVenue(
//        Guid venueId,
//        Guid photoId,
//        CancellationToken cancellationToken)
//    {
//        var command = new RemovePhotoFromVenueCommand(venueId, photoId);
//        await _mediator.Send(command, cancellationToken);
//        return NoContent();
//    }

//    /// <summary>
//    /// Dosya upload ederek mekana fotoğraf ekler.
//    /// </summary>
//    [HttpPost("{venueId:guid}/photos/upload")]
//    public async Task<IActionResult> UploadVenuePhoto(
//        Guid venueId,
//        IFormFile file,
//        [FromForm] string? caption,
//        [FromForm] int? sortOrder,
//        CancellationToken cancellationToken)
//    {
//        if (file == null || file.Length == 0)
//            return BadRequest("File is required.");

//        var command = new UploadVenuePhotoCommand(
//            venueId,
//            file.OpenReadStream(),
//            file.FileName,
//            file.ContentType ?? "application/octet-stream",
//            caption,
//            sortOrder);

//        var photoId = await _mediator.Send(command, cancellationToken);

//        return CreatedAtAction(
//            nameof(GetPhotosForVenue),
//            new { venueId },
//            new { id = photoId });
//    }


//    /// <summary>
//    /// Mekanın menüsünü (kategori + item'lar) döner.
//    /// </summary>
//    [HttpGet("{venueId:guid}/menu")]
//    public async Task<ActionResult<IReadOnlyList<MenuCategoryDto>>> GetMenuForVenue(
//        Guid venueId,
//        CancellationToken cancellationToken)
//    {
//        var query = new GetMenuForVenueQuery(venueId);

//        var result = await _mediator.Send(query, cancellationToken);

//        return Ok(result);
//    }



//    /// <summary>
//    /// Mekana yeni menü kategorisi ekler.
//    /// </summary>
//    [HttpPost("{venueId:guid}/menu-categories")]
//    public async Task<IActionResult> CreateMenuCategory(
//        Guid venueId,
//        [FromBody] CreateMenuCategoryRequest request,
//        CancellationToken cancellationToken)
//    {
//        var command = new CreateMenuCategoryCommand(
//            venueId,
//            request.Name,
//            request.SortOrder
//        );

//        var menuCategoryId = await _mediator.Send(command, cancellationToken);

//        return CreatedAtAction(
//            nameof(GetMenuForVenue),
//            new { venueId },
//            new { id = menuCategoryId });
//    }

//    /// <summary>
//    /// Menü kategorisini günceller.
//    /// </summary>
//    [HttpPut("menu-categories/{menuCategoryId:int}")]
//    public async Task<IActionResult> UpdateMenuCategory(
//        int menuCategoryId,
//        [FromBody] UpdateMenuCategoryRequest request,
//        CancellationToken cancellationToken)
//    {
//        var command = new UpdateMenuCategoryCommand(
//            menuCategoryId,
//            request.Name,
//            request.SortOrder,
//            request.IsActive
//        );

//        await _mediator.Send(command, cancellationToken);

//        return NoContent();
//    }

//    /// <summary>
//    /// Menü kategorisini siler.
//    /// </summary>
//    [HttpDelete("menu-categories/{menuCategoryId:int}")]
//    public async Task<IActionResult> DeleteMenuCategory(
//        int menuCategoryId,
//        CancellationToken cancellationToken)
//    {
//        var command = new DeleteMenuCategoryCommand(menuCategoryId);

//        await _mediator.Send(command, cancellationToken);

//        return NoContent();
//    }


//    /// <summary>
//    /// Menü kategorisine yeni menü ürünü ekler.
//    /// </summary>
//    [HttpPost("menu-categories/{menuCategoryId:int}/items")]
//    public async Task<IActionResult> CreateMenuItem(
//        int menuCategoryId,
//        [FromBody] CreateMenuItemRequest request,
//        CancellationToken cancellationToken)
//    {
//        var command = new CreateMenuItemCommand(
//            menuCategoryId,
//            request.Name,
//            request.Description,
//            request.Price,
//            request.ImageUrl,
//            request.SortOrder
//        );

//        var itemId = await _mediator.Send(command, cancellationToken);

//        // CreatedAtAction yerine doğrudan Created döndürüyoruz
//        return Created("", new { id = itemId });
//    }



//    /// <summary>
//    /// Menü ürününü günceller.
//    /// </summary>
//    [HttpPut("menu-items/{menuItemId:guid}")]
//    public async Task<IActionResult> UpdateMenuItem(
//        Guid menuItemId,
//        [FromBody] UpdateMenuItemRequest request,
//        CancellationToken cancellationToken)
//    {
//        var command = new UpdateMenuItemCommand(
//            menuItemId,
//            request.Name,
//            request.Description,
//            request.Price,
//            request.ImageUrl,
//            request.IsAvailable,
//            request.SortOrder
//        );

//        await _mediator.Send(command, cancellationToken);

//        return NoContent();
//    }
//    /// <summary>
//    /// Menü ürününü siler.
//    /// </summary>
//    [HttpDelete("menu-items/{menuItemId:guid}")]
//    public async Task<IActionResult> DeleteMenuItem(
//        Guid menuItemId,
//        CancellationToken cancellationToken)
//    {
//        var command = new DeleteMenuItemCommand(menuItemId);

//        await _mediator.Send(command, cancellationToken);

//        return NoContent();
//    }

//    [HttpPost("menu-items/{menuItemId:guid}/photo-upload")]
//    public async Task<IActionResult> UploadMenuItemPhoto(
//    Guid menuItemId,
//    IFormFile file,
//    CancellationToken cancellationToken)
//    {
//        if (file == null || file.Length == 0)
//            return BadRequest("File is required.");

//        var command = new UploadMenuItemPhotoCommand(
//            menuItemId,
//            file.OpenReadStream(),
//            file.FileName,
//            file.ContentType ?? "application/octet-stream");

//        await _mediator.Send(command, cancellationToken);

//        return NoContent();
//    }

//    /// <summary>
//    /// Mekanın aktif etkinliklerini döner.
//    /// </summary>
//    [HttpGet("{venueId:guid}/events")]
//    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetEventsForVenue(
//        Guid venueId,
//        CancellationToken cancellationToken)
//    {
//        var query = new GetVenueEventsQuery(venueId);
//        var result = await _mediator.Send(query, cancellationToken);
//        return Ok(result);
//    }

//    /// <summary>
//    /// Mekana yeni etkinlik ekler.
//    /// </summary>
//    [HttpPost("{venueId:guid}/events")]
//    public async Task<IActionResult> CreateEvent(
//        Guid venueId,
//        [FromBody] CreateEventRequest request,
//        CancellationToken cancellationToken)
//    {
//        var command = new CreateEventCommand(
//            venueId,
//            request.Title,
//            request.Description,
//            request.StartDate,
//            request.EndDate,
//            request.ImageUrl);

//        var eventId = await _mediator.Send(command, cancellationToken);

//        return Created("", new { id = eventId });
//    }

//    /// <summary>
//    /// Etkinliği günceller.
//    /// </summary>
//    [HttpPut("events/{eventId:guid}")]
//    public async Task<IActionResult> UpdateEvent(
//        Guid eventId,
//        [FromBody] UpdateEventRequest request,
//        CancellationToken cancellationToken)
//    {
//        var command = new UpdateEventCommand(
//            eventId,
//            request.Title,
//            request.Description,
//            request.StartDate,
//            request.EndDate,
//            request.ImageUrl,
//            request.IsActive);

//        await _mediator.Send(command, cancellationToken);

//        return NoContent();
//    }

//    /// <summary>
//    /// Etkinliği siler.
//    /// </summary>
//    [HttpDelete("events/{eventId:guid}")]
//    public async Task<IActionResult> DeleteEvent(
//        Guid eventId,
//        CancellationToken cancellationToken)
//    {
//        var command = new DeleteEventCommand(eventId);

//        await _mediator.Send(command, cancellationToken);

//        return NoContent();
//    }

//    /// <summary>
//    /// Etkinliğe dosya upload ederek görsel ekler / günceller.
//    /// </summary>
//    [HttpPost("events/{eventId:guid}/image-upload")]
//    public async Task<IActionResult> UploadEventImage(
//        Guid eventId,
//        IFormFile file,
//        CancellationToken cancellationToken)
//    {
//        if (file == null || file.Length == 0)
//            return BadRequest("File is required.");

//        var command = new UploadEventImageCommand(
//            eventId,
//            file.OpenReadStream(),
//            file.FileName,
//            file.ContentType ?? "application/octet-stream"
//        );

//        await _mediator.Send(command, cancellationToken);

//        return NoContent();
//    }


//    // GET /api/venues/search
//    [HttpGet("search")]
//    public async Task<IActionResult> Search(
//        [FromQuery] double lat,
//        [FromQuery] double lon,
//        [FromQuery] double radius = 2000,
//        [FromQuery] int? categoryId = null,
//        [FromQuery] byte? minPriceLevel = null,
//        [FromQuery] byte? maxPriceLevel = null,
//        [FromQuery] bool? openNow = null,
//        CancellationToken cancellationToken = default)
//    {
//        var query = new SearchVenuesQuery(
//            lat,
//            lon,
//            radius,
//            categoryId,
//            minPriceLevel,
//            maxPriceLevel,
//            openNow);

//        var result = await _mediator.Send(query, cancellationToken);

//        return Ok(result);
//    }

//    /// <summary>
//    /// Giriş yapmış kullanıcının mekanına ait aktif etkinlikleri döner.
//    /// </summary>
//    [HttpGet("my/events")]
//    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetMyEvents(
//        CancellationToken cancellationToken)
//    {
//        var ownerUserId = GetCurrentUserId();

//        var venue = await _venueRepository.GetByOwnerIdAsync(ownerUserId, cancellationToken);
//        if (venue is null)
//            return NotFound("Bu kullanıcıya ait bir mekan bulunamadı.");

//        var query = new GetVenueEventsQuery(venue.Id);

//        var result = await _mediator.Send(query, cancellationToken);

//        return Ok(result);
//    }






//}

using CityDiscovery.Venues.API.Models.Requests;
using CityDiscovery.Venues.Application.Features.Venues.Commands.ActivateVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.ApproveVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.CreateVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.DeactivateVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateVenueBasicInfo;
using CityDiscovery.Venues.Application.Features.Venues.Queries.GetNearbyVenues;
using CityDiscovery.Venues.Application.Features.Venues.Queries.SearchVenues;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Application.Interfaces.Services;
using CityDiscovery.Venues.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CityDiscovery.Venues.API.Controllers;

/// <summary>
/// Mekan yönetimi endpoint'leri - Temel CRUD işlemleri, arama ve onaylama
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // default: tüm endpoint'ler auth ister, aşağıda [AllowAnonymous] ile açacağız
[Tags("Venues")]
public class VenuesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IVenueRepository _venueRepository;
    private readonly ICurrentUserService _currentUser;

    public VenuesController(
        IMediator mediator,
        IVenueRepository venueRepository,
        ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _venueRepository = venueRepository;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Giriş yapmış kullanıcının sahip olduğu mekanı döner
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kullanıcının mekanı</returns>
    /// <response code="200">Başarılı - Mekan bilgileri döner</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok (Owner değilsiniz)</response>
    /// <response code="404">Kullanıcıya ait mekan bulunamadı</response>
    [HttpGet("my")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyVenue(CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        var venue = await _venueRepository.GetByOwnerIdAsync(
            _currentUser.UserId.Value,
            cancellationToken);

        if (venue is null)
            return NotFound("Bu kullanıcıya ait bir mekan bulunamadı.");

        return Ok(MapToDto(venue));
    }

    /// <summary>
    /// Yeni mekan oluşturur (Sadece Owner)
    /// </summary>
    /// <param name="request">Mekan bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Oluşturulan mekan ID</returns>
    /// <response code="201">Başarılı - Mekan oluşturuldu</response>
    /// <response code="400">Geçersiz istek (örnek: Owner zaten bir mekana sahip)</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok (Owner değilsiniz)</response>
    /// <remarks>
    /// Örnek istek:
    /// 
    /// POST /api/Venues
    /// {
    ///   "name": "Lezzet Durağı",
    ///   "description": "Geleneksel Türk mutfağı",
    ///   "addressText": "İstiklal Caddesi No:123, Beyoğlu, İstanbul",
    ///   "phone": "+905551234567",
    ///   "websiteUrl": "https://www.lezzetduragi.com",
    ///   "priceLevel": 3,
    ///   "openingHoursJson": "{\"Mon\":\"09:00-22:00\",\"Tue\":\"09:00-22:00\",\"Wed\":\"09:00-22:00\",\"Thu\":\"09:00-22:00\",\"Fri\":\"09:00-23:00\",\"Sat\":\"10:00-23:00\",\"Sun\":\"10:00-22:00\"}",
    ///   "latitude": 41.0082,
    ///   "longitude": 28.9784
    /// }
    /// 
    /// Not: OwnerUserId token'dan otomatik alınır, request body'de gönderilmez.
    /// PriceLevel değerleri: 1=Çok Ucuz, 2=Ucuz, 3=Orta, 4=Pahalı, 5=Çok Pahalı
    /// </remarks>
    [HttpPost]
    [Authorize(Policy = "OwnerOnly")]
    [SwaggerOperation(
        Summary = "Yeni mekan oluşturur",
        Description = "Owner rolündeki kullanıcılar yeni mekan oluşturabilir. OwnerUserId token'dan otomatik alınır.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateVenue(
        [FromBody] CreateVenueRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        var command = new CreateVenueCommand(
            _currentUser.UserId.Value, // Owner id token'dan
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

        return CreatedAtAction(nameof(GetById), new { id = venueId }, new { id = venueId });

    }

    /// <summary>
    /// Mekan günceller (Sadece kendi mekanını güncelleyebilir veya Admin)
    /// </summary>
    /// <param name="id">Mekan ID</param>
    /// <param name="request">Güncellenecek mekan bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Mekan güncellendi</response>
    /// <response code="400">Geçersiz istek</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok (Başkasının mekanını güncelleyemezsiniz)</response>
    /// <response code="404">Mekan bulunamadı</response>
    /// <remarks>
    /// Örnek istek:
    /// 
    /// PUT /api/Venues/{id}
    /// {
    ///   "name": "Lezzet Durağı - Güncellenmiş",
    ///   "description": "Geleneksel Türk mutfağı ve özel lezzetler",
    ///   "addressText": "İstiklal Caddesi No:123, Beyoğlu, İstanbul",
    ///   "phone": "+905551234567",
    ///   "websiteUrl": "https://www.lezzetduragi.com",
    ///   "priceLevel": 3,
    ///   "openingHoursJson": "{\"Mon\":\"09:00-22:00\",\"Tue\":\"09:00-22:00\",\"Wed\":\"09:00-22:00\",\"Thu\":\"09:00-22:00\",\"Fri\":\"09:00-23:00\",\"Sat\":\"10:00-23:00\",\"Sun\":\"10:00-22:00\"}",
    ///   "latitude": 41.0082,
    ///   "longitude": 28.9784
    /// }
    /// 
    /// PriceLevel değerleri: 1=Çok Ucuz, 2=Ucuz, 3=Orta, 4=Pahalı, 5=Çok Pahalı
    /// </remarks>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "OwnerOnly")]
    [SwaggerOperation(
        Summary = "Mekan günceller",
        Description = "Owner kendi mekanını, Admin herhangi bir mekanı güncelleyebilir.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVenue(
        Guid id,
        [FromBody] UpdateVenueRequest request,
        CancellationToken cancellationToken)
    {
        // Owner değilse ek kontrol
        if (!_currentUser.IsAdmin)
        {
            var venue = await _venueRepository.GetByIdAsync(id, cancellationToken);

            if (venue == null)
                return NotFound();

            if (!_currentUser.UserId.HasValue || venue.OwnerUserId != _currentUser.UserId.Value)
                return Forbid();
        }

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

    /// <summary>
    /// Mekan onaylar (Sadece Admin)
    /// </summary>
    /// <param name="id">Mekan ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Mekan onaylandı</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok (Admin değilsiniz)</response>
    /// <response code="404">Mekan bulunamadı</response>
    [HttpPut("{id:guid}/approve")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new ApproveVenueCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while approving the venue.", details = ex.Message });
        }
    }

    /// <summary>
    /// Mekanı siler (Sadece Admin veya Owner)
    /// </summary>
    /// <param name="id">Mekan ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Mekan silindi</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok (Başkasının mekanını silemezsiniz)</response>
    /// <response code="404">Mekan bulunamadı</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVenue(Guid id, CancellationToken cancellationToken)
    {
        // Owner değilse ek kontrol (Admin olabilir)
        if (!_currentUser.IsAdmin)
        {
            var venue = await _venueRepository.GetByIdAsync(id, cancellationToken);

            if (venue == null)
                return NotFound();

            if (!_currentUser.UserId.HasValue || venue.OwnerUserId != _currentUser.UserId.Value)
                return Forbid();
        }

        try
        {
            var command = new DeleteVenueCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting the venue.", details = ex.Message });
        }
    }

    /// <summary>
    /// Mekan bilgilerini getirir (Herkes görebilir - Public endpoint)
    /// </summary>
    /// <param name="id">Mekan ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Mekan bilgileri</returns>
    /// <response code="200">Başarılı - Mekan bilgileri döner</response>
    /// <response code="404">Mekan bulunamadı</response>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(id, cancellationToken);

        if (venue is null)
            return NotFound();

        return Ok(MapToDto(venue));
    }

    /// <summary>
    /// Mekanın temel bilgilerini getirir (Diğer servisler için - Public endpoint)
    /// </summary>
    [HttpGet("{id:guid}/basic")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVenueBasic(Guid id, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(id, cancellationToken);

        if (venue is null)
            return NotFound();

        var dto = new CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue.VenueBasicDto
        {
            Id = venue.Id,
            Name = venue.Name,
            AddressText = venue.AddressText,
            PriceLevel = venue.PriceLevel?.Value,
            Location = new CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue.LocationDto
            {
                Latitude = venue.Location.Y,
                Longitude = venue.Location.X
            }
        };

        return Ok(dto);
    }

    /// <summary>
    /// Owner'a ait mekanı getirir (Diğer servisler için - Public endpoint)
    /// </summary>
    [HttpGet("by-owner/{ownerId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVenueByOwner(Guid ownerId, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByOwnerIdAsync(ownerId, cancellationToken);

        if (venue is null)
            return NotFound();

        return Ok(MapToDto(venue));
    }

    /// <summary>
    /// Mekan var mı kontrol eder (Diğer servisler için - Public endpoint)
    /// </summary>
    [HttpGet("{id:guid}/exists")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckVenueExists(Guid id, CancellationToken cancellationToken)
    {
        var exists = await _venueRepository.ExistsAsync(id, cancellationToken);
        return Ok(exists);
    }

    /// <summary>
    /// Birden fazla mekanı toplu olarak getirir (Diğer servisler için - Public endpoint)
    /// </summary>
    [HttpGet("batch")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVenuesBatch(
        [FromQuery] List<Guid> venueIds,
        CancellationToken cancellationToken)
    {
        if (venueIds == null || !venueIds.Any())
            return BadRequest("venueIds parameter is required.");

        var venues = await _venueRepository.GetByIdsAsync(venueIds, cancellationToken);

        var dtos = venues.Select(v => new CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue.VenueBasicDto
        {
            Id = v.Id,
            Name = v.Name,
            AddressText = v.AddressText,
            PriceLevel = v.PriceLevel?.Value,
            Location = new CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue.LocationDto
            {
                Latitude = v.Location.Y,
                Longitude = v.Location.X
            }
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Yakındaki mekanları getirir (Public endpoint)
    /// </summary>
    /// <param name="lat">Enlem (Latitude)</param>
    /// <param name="lon">Boylam (Longitude)</param>
    /// <param name="radius">Arama yarıçapı (metre, varsayılan: 2000)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yakındaki mekanlar listesi</returns>
    /// <response code="200">Başarılı - Mekan listesi döner</response>
    [HttpGet("nearby")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
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

    private object MapToDto(Venuex venue)
    {
        return new
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
                Latitude = venue.Location.Y,
                Longitude = venue.Location.X
            },
            venue.IsApproved,
            venue.IsActive,
            venue.CreatedAt
        };
    }

    // Aktivasyon / Deaktivasyon

    [HttpPut("{id:guid}/activate")]
    [Authorize(Policy = "OwnerOnly")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateVenueCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/deactivate")]
    [Authorize(Policy = "OwnerOnly")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateVenueCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Mekanları arama yapar (kategori, fiyat seviyesi, açık olan mekanlar)
    /// </summary>
    /// <param name="lat">Enlem (Latitude)</param>
    /// <param name="lon">Boylam (Longitude)</param>
    /// <param name="radius">Arama yarıçapı (metre, varsayılan: 2000)</param>
    /// <param name="categoryId">Kategori ID (opsiyonel)</param>
    /// <param name="minPriceLevel">Minimum fiyat seviyesi (1-5, opsiyonel)</param>
    /// <param name="maxPriceLevel">Maximum fiyat seviyesi (1-5, opsiyonel)</param>
    /// <param name="openNow">Şu an açık olan mekanlar (opsiyonel)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bulunan mekanlar listesi</returns>
    /// <response code="200">Başarılı - Mekan listesi döner</response>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] double lat,
        [FromQuery] double lon,
        [FromQuery] double radius = 2000,
        [FromQuery] int? categoryId = null,
        [FromQuery] byte? minPriceLevel = null,
        [FromQuery] byte? maxPriceLevel = null,
        [FromQuery] bool? openNow = null,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchVenuesQuery(
            lat,
            lon,
            radius,
            categoryId,
            minPriceLevel,
            maxPriceLevel,
            openNow);

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

}
