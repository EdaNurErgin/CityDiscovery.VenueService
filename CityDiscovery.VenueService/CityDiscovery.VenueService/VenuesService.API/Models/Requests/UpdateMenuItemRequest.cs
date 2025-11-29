using System.ComponentModel.DataAnnotations;

namespace CityDiscovery.Venues.API.Models.Requests;

/// <summary>
/// Menü ürünü güncelleme isteği
/// </summary>
/// <example>
/// {
///   "name": "Espresso - Güncellenmiş",
///   "description": "Geleneksel İtalyan espresso",
///   "price": 30.00,
///   "imageUrl": "https://example.com/photos/espresso.jpg",
///   "isAvailable": true,
///   "sortOrder": 1
/// }
/// </example>
public sealed class UpdateMenuItemRequest
{
    /// <summary>
    /// Ürün adı
    /// </summary>
    /// <example>Espresso - Güncellenmiş</example>
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
    /// <example>30.00</example>
    [Range(0, 999999.99, ErrorMessage = "Fiyat 0 ile 999999.99 arasında olmalıdır")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Ürün görsel URL'i (opsiyonel)
    /// </summary>
    /// <example>https://example.com/photos/espresso.jpg</example>
    [Url(ErrorMessage = "Geçerli bir URL formatı giriniz")]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Ürün mevcut mu?
    /// </summary>
    /// <example>true</example>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Sıralama (kategoride gösterilme sırası)
    /// </summary>
    /// <example>1</example>
    [Range(0, int.MaxValue, ErrorMessage = "Sıralama 0 veya pozitif bir sayı olmalıdır")]
    public int SortOrder { get; set; }
}
