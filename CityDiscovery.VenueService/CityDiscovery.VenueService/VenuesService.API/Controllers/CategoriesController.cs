using CityDiscovery.Venues.API.Models.Requests;
using CityDiscovery.Venues.Application.Features.Categories.Commands.CreateCategory;
using CityDiscovery.Venues.Application.Features.Categories.Queries.GetAllCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CityDiscovery.Venues.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Sistemdeki tüm aktif mekan kategorilerini döner.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Yeni bir mekan kategorisi oluşturur (Sadece Admin yetkisi gerektirir).
    /// </summary>
    /// <remarks>
    /// Sistemde aynı isme sahip başka bir kategori varsa işlem başarısız olur.
    /// İşlemi gerçekleştirebilmek için JWT token içinde 'Admin' rolünün bulunması zorunludur.
    /// </remarks>
    /// <param name="request">Eklenecek yeni kategorinin bilgileri (İsim, İkon).</param>
    /// <param name="cancellationToken">İptal tokenı.</param>
    /// <returns>Başarı mesajı ve oluşturulan kategorinin ID değerini döner.</returns>
    /// <response code="200">Kategori başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçerli değil (Örn: İsim boş gönderildi veya aynı isimde kategori zaten var).</response>
    /// <response code="401">Kullanıcı kimlik doğrulaması (Token) başarısız.</response>
    /// <response code="403">Kullanıcının bu işlemi yapmak için Admin yetkisi yok.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized("Geçersiz kullanıcı kimliği.");

        var command = new CreateCategoryCommand(
                request.Name,
                request.IconUrl,
                userId,
                userRole!);

        var categoryId = await _mediator.Send(command, cancellationToken);

        // Kategori başarıyla eklendi döndür
        return Ok(new { Message = "Kategori başarıyla oluşturuldu.", CategoryId = categoryId });
    }
}
