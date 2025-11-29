using System.ComponentModel.DataAnnotations;

namespace CityDiscovery.Venues.API.Models.Requests;

/// <summary>
/// Menü ürünü oluşturma isteği
/// </summary>
/// <example>
/// {
///   "name": "Espresso",
///   "description": "Geleneksel İtalyan espresso",
///   "price": 25.50,
///   "imageUrl": "https://example.com/photos/espresso.jpg",
///   "isAvailable": true,
///   "sortOrder": 1
/// }
/// </example>
public sealed class CreateMenuItemRequest
{
    /// <summary>
    /// Ürün adı
    /// </summary>
    /// <example>Espresso</example>
    [Required(ErrorMessage = "Ürün adı zorunludur")]
    [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Ürün açıklaması (opsiyonel)
    /// </summary>
    /// <example>Geleneksel İtalyan espresso</example>
    [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
    public string? Description { get; set; }

    /// <summary>
    /// Ürün fiyatı (opsiyonel, fiyatsız ürünler için null)
    /// </summary>
    /// <example>25.50</example>
    [Range(0, 999999.99, ErrorMessage = "Fiyat 0 ile 999999.99 arasında olmalıdır")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Ürün görsel URL'i (opsiyonel)
    /// </summary>
    /// <example>https://example.com/photos/espresso.jpg</example>
    [Url(ErrorMessage = "Geçerli bir URL formatı giriniz")]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Ürün mevcut mu? (varsayılan: true)
    /// </summary>
    /// <example>true</example>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Sıralama (opsiyonel, kategoride gösterilme sırası)
    /// </summary>
    /// <example>1</example>
    [Range(0, int.MaxValue, ErrorMessage = "Sıralama 0 veya pozitif bir sayı olmalıdır")]
    public int? SortOrder { get; set; }
}
