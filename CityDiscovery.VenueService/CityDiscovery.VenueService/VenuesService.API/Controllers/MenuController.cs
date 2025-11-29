using CityDiscovery.Venues.Application.Features.Venues.Commands.CreateMenuCategory;
using CityDiscovery.Venues.Application.Features.Venues.Commands.CreateMenuItem;
using CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteMenuCategory;
using CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteMenuItem;
using CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateMenuCategory;
using CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateMenuItem;
using CityDiscovery.Venues.Application.Features.Venues.Commands.UploadMenuItemPhoto;
using CityDiscovery.Venues.Application.Features.Venues.Queries.GetMenuForVenue;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Application.Interfaces.Services;
using CityDiscovery.Venues.API.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CityDiscovery.Venues.API.Controllers;

/// <summary>
/// Mekan menü yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/venues")]
[Authorize]
[Tags("Menu")]
public class MenuController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IVenueRepository _venueRepository;
    private readonly ICurrentUserService _currentUser;

    public MenuController(
        IMediator mediator,
        IVenueRepository venueRepository,
        ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _venueRepository = venueRepository;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Mekanın menüsünü getirir (kategori + item'lar)
    /// </summary>
    /// <param name="venueId">Mekan ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Menü kategorileri ve item'ları</returns>
    /// <response code="200">Başarılı - Menü listesi döner</response>
    /// <response code="404">Mekan bulunamadı</response>
    [HttpGet("{venueId:guid}/menu")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<MenuCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<MenuCategoryDto>>> GetMenu(
        Guid venueId,
        CancellationToken cancellationToken)
    {
        var query = new GetMenuForVenueQuery(venueId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Giriş yapmış kullanıcının mekanının menüsünü getirir
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Menü kategorileri ve item'ları</returns>
    /// <response code="200">Başarılı - Menü listesi döner</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="404">Mekan bulunamadı</response>
    [HttpGet("my/menu")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(typeof(IReadOnlyList<MenuCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<MenuCategoryDto>>> GetMyMenu(
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        var venue = await _venueRepository.GetByOwnerIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (venue is null)
            return NotFound("Bu kullanıcıya ait bir mekan bulunamadı.");

        var query = new GetMenuForVenueQuery(venue.Id);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Mekana yeni menü kategorisi ekler
    /// </summary>
    /// <param name="venueId">Mekan ID</param>
    /// <param name="request">Menü kategorisi bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Oluşturulan kategori ID</returns>
    /// <response code="201">Başarılı - Kategori oluşturuldu</response>
    /// <response code="400">Geçersiz istek</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <remarks>
    /// Örnek istek:
    /// 
    /// POST /api/venues/{venueId}/menu-categories
    /// {
    ///   "name": "Kahveler",
    ///   "sortOrder": 1
    /// }
    /// </remarks>
    [HttpPost("{venueId:guid}/menu-categories")]
    [Authorize(Policy = "OwnerOnly")]
    [SwaggerOperation(
        Summary = "Mekana yeni menü kategorisi ekler",
        Description = "Owner rolündeki kullanıcılar mekana menü kategorisi ekleyebilir.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateMenuCategory(
        Guid venueId,
        [FromBody] CreateMenuCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateMenuCategoryCommand(
            venueId,
            request.Name,
            request.SortOrder
        );

        var menuCategoryId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetMenu),
            new { venueId },
            new { id = menuCategoryId });
    }

    /// <summary>
    /// Menü kategorisini günceller
    /// </summary>
    /// <param name="menuCategoryId">Menü kategori ID</param>
    /// <param name="request">Güncellenecek bilgiler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Kategori güncellendi</response>
    /// <response code="400">Geçersiz istek</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <response code="404">Kategori bulunamadı</response>
    [HttpPut("menu-categories/{menuCategoryId:int}")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMenuCategory(
        int menuCategoryId,
        [FromBody] UpdateMenuCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMenuCategoryCommand(
            menuCategoryId,
            request.Name,
            request.SortOrder,
            request.IsActive
        );

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Menü kategorisini siler
    /// </summary>
    /// <param name="menuCategoryId">Menü kategori ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Kategori silindi</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <response code="404">Kategori bulunamadı</response>
    [HttpDelete("menu-categories/{menuCategoryId:int}")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMenuCategory(
        int menuCategoryId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteMenuCategoryCommand(menuCategoryId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Menü kategorisine yeni ürün ekler
    /// </summary>
    /// <param name="menuCategoryId">Menü kategori ID</param>
    /// <param name="request">Ürün bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Oluşturulan ürün ID</returns>
    /// <response code="201">Başarılı - Ürün oluşturuldu</response>
    /// <response code="400">Geçersiz istek</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <remarks>
    /// Örnek istek:
    /// 
    /// POST /api/venues/menu-categories/{menuCategoryId}/items
    /// {
    ///   "name": "Espresso",
    ///   "description": "Geleneksel İtalyan espresso",
    ///   "price": 25.50,
    ///   "imageUrl": "https://example.com/photos/espresso.jpg",
    ///   "isAvailable": true,
    ///   "sortOrder": 1
    /// }
    /// </remarks>
    [HttpPost("menu-categories/{menuCategoryId:int}/items")]
    [Authorize(Policy = "OwnerOnly")]
    [SwaggerOperation(
        Summary = "Menü kategorisine yeni ürün ekler",
        Description = "Owner rolündeki kullanıcılar menü kategorisine ürün ekleyebilir.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateMenuItem(
        int menuCategoryId,
        [FromBody] CreateMenuItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateMenuItemCommand(
            menuCategoryId,
            request.Name,
            request.Description,
            request.Price,
            request.ImageUrl,
            request.SortOrder
        );

        var itemId = await _mediator.Send(command, cancellationToken);
        return Created("", new { id = itemId });
    }

    /// <summary>
    /// Menü ürününü günceller
    /// </summary>
    /// <param name="menuItemId">Menü ürün ID</param>
    /// <param name="request">Güncellenecek bilgiler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Ürün güncellendi</response>
    /// <response code="400">Geçersiz istek</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <response code="404">Ürün bulunamadı</response>
    [HttpPut("menu-items/{menuItemId:guid}")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMenuItem(
        Guid menuItemId,
        [FromBody] UpdateMenuItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMenuItemCommand(
            menuItemId,
            request.Name,
            request.Description,
            request.Price,
            request.ImageUrl,
            request.IsAvailable,
            request.SortOrder
        );

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Menü ürününü siler
    /// </summary>
    /// <param name="menuItemId">Menü ürün ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Ürün silindi</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <response code="404">Ürün bulunamadı</response>
    [HttpDelete("menu-items/{menuItemId:guid}")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMenuItem(
        Guid menuItemId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteMenuItemCommand(menuItemId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Menü ürününe fotoğraf yükler
    /// </summary>
    /// <param name="menuItemId">Menü ürün ID</param>
    /// <param name="file">Yüklenecek fotoğraf dosyası</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Fotoğraf yüklendi</response>
    /// <response code="400">Geçersiz dosya</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    [HttpPost("menu-items/{menuItemId:guid}/photo-upload")]
    [Authorize(Policy = "OwnerOnly")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UploadMenuItemPhoto(
        Guid menuItemId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        var command = new UploadMenuItemPhotoCommand(
            menuItemId,
            file.OpenReadStream(),
            file.FileName,
            file.ContentType ?? "application/octet-stream");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}

