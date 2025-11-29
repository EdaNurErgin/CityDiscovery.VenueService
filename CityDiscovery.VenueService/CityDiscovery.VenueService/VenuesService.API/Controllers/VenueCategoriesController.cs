using CityDiscovery.Venues.Application.Features.Venues.Commands.AddCategoryToVenue;
using CityDiscovery.Venues.Application.Features.Venues.Commands.RemoveCategoryFromVenue;
using CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenueCategories;
using CityDiscovery.Venues.API.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CityDiscovery.Venues.Application.Features.Categories.Queries.GetAllCategories;
using CityDiscovery.VenueService.VenuesService.API.Models.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace CityDiscovery.Venues.API.Controllers;

/// <summary>
/// Mekan kategori yönetimi endpoint'leri
/// </summary>
[ApiController]
[Route("api/venues/{venueId:guid}/categories")]
[Authorize]
[Tags("Venue Categories")]
public class VenueCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VenueCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Mekanın kategorilerini listeler
    /// </summary>
    /// <param name="venueId">Mekan ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Mekan kategorileri listesi</returns>
    /// <response code="200">Başarılı - Kategori listesi döner</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(
        Guid venueId,
        CancellationToken cancellationToken)
    {
        var query = new GetVenueCategoriesQuery(venueId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Mekana kategori ekler
    /// </summary>
    /// <param name="venueId">Mekan ID</param>
    /// <param name="request">Kategori bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Kategori eklendi</response>
    /// <response code="400">Geçersiz istek</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <remarks>
    /// Örnek istek:
    /// 
    /// POST /api/venues/{venueId}/categories
    /// {
    ///   "categoryId": 1
    /// }
    /// 
    /// Kategori ID'lerini görmek için GET /api/Categories endpoint'ini kullanabilirsiniz.
    /// </remarks>
    [HttpPost]
    [Authorize(Policy = "OwnerOnly")]
    [SwaggerOperation(
        Summary = "Mekana kategori ekler",
        Description = "Owner rolündeki kullanıcılar mekana kategori ekleyebilir.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddCategory(
        Guid venueId,
        [FromBody] AddVenueCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCategoryToVenueCommand(venueId, request.CategoryId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Mekandan kategori kaldırır
    /// </summary>
    /// <param name="venueId">Mekan ID</param>
    /// <param name="categoryId">Kaldırılacak kategori ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Başarı durumu</returns>
    /// <response code="204">Başarılı - Kategori kaldırıldı</response>
    /// <response code="401">Yetkisiz erişim</response>
    /// <response code="403">Bu işlem için yetkiniz yok</response>
    /// <response code="404">Kategori bulunamadı</response>
    [HttpDelete("{categoryId:int}")]
    [Authorize(Policy = "OwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveCategory(
        Guid venueId,
        int categoryId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveCategoryFromVenueCommand(venueId, categoryId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}

