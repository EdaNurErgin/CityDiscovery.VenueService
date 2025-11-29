using System.ComponentModel.DataAnnotations;

namespace CityDiscovery.Venues.API.Models.Requests;

/// <summary>
/// Menü kategorisi oluşturma isteği
/// </summary>
/// <example>
/// {
///   "name": "Kahveler",
///   "sortOrder": 1
/// }
/// </example>
public sealed class CreateMenuCategoryRequest
{
    /// <summary>
    /// Kategori adı
    /// </summary>
    /// <example>Kahveler</example>
    [Required(ErrorMessage = "Kategori adı zorunludur")]
    [StringLength(200, ErrorMessage = "Kategori adı en fazla 200 karakter olabilir")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Sıralama (opsiyonel, menüde gösterilme sırası)
    /// </summary>
    /// <example>1</example>
    [Range(0, int.MaxValue, ErrorMessage = "Sıralama 0 veya pozitif bir sayı olmalıdır")]
    public int? SortOrder { get; set; }
}
