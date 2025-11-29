using System.ComponentModel.DataAnnotations;

namespace CityDiscovery.Venues.API.Models.Requests;

/// <summary>
/// Menü kategorisi güncelleme isteği
/// </summary>
/// <example>
/// {
///   "name": "Kahveler - Güncellenmiş",
///   "sortOrder": 1,
///   "isActive": true
/// }
/// </example>
public sealed class UpdateMenuCategoryRequest
{
    /// <summary>
    /// Kategori adı
    /// </summary>
    /// <example>Kahveler - Güncellenmiş</example>
    [Required(ErrorMessage = "Kategori adı zorunludur")]
    [StringLength(200, ErrorMessage = "Kategori adı en fazla 200 karakter olabilir")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Sıralama (menüde gösterilme sırası)
    /// </summary>
    /// <example>1</example>
    [Range(0, int.MaxValue, ErrorMessage = "Sıralama 0 veya pozitif bir sayı olmalıdır")]
    public int SortOrder { get; set; }

    /// <summary>
    /// Kategori aktif mi?
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; set; }
}
