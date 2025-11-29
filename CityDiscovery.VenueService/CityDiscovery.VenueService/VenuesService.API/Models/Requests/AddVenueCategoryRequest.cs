using System.ComponentModel.DataAnnotations;

namespace CityDiscovery.VenueService.VenuesService.API.Models.Requests;

/// <summary>
/// Mekana kategori ekleme isteği
/// </summary>
/// <example>
/// {
///   "categoryId": 1
/// }
/// </example>
public sealed class AddVenueCategoryRequest
{
    /// <summary>
    /// Eklenecek kategori ID
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "Kategori ID zorunludur")]
    [Range(1, int.MaxValue, ErrorMessage = "Kategori ID pozitif bir sayı olmalıdır")]
    public int CategoryId { get; set; }
}
