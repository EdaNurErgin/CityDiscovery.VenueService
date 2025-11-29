using System.ComponentModel.DataAnnotations;

namespace CityDiscovery.Venues.API.Models.Requests;

/// <summary>
/// Mekana fotoğraf ekleme isteği
/// </summary>
/// <example>
/// {
///   "url": "https://example.com/photos/venue-photo.jpg",
///   "caption": "Mekanın dış görünümü",
///   "sortOrder": 1
/// }
/// </example>
public sealed class AddVenuePhotoRequest
{
    /// <summary>
    /// Fotoğraf URL'i
    /// </summary>
    /// <example>https://example.com/photos/venue-photo.jpg</example>
    [Required(ErrorMessage = "Fotoğraf URL'i zorunludur")]
    [Url(ErrorMessage = "Geçerli bir URL formatı giriniz")]
    public string Url { get; set; } = default!;

    /// <summary>
    /// Fotoğraf açıklaması (opsiyonel)
    /// </summary>
    /// <example>Mekanın dış görünümü</example>
    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
    public string? Caption { get; set; }

    /// <summary>
    /// Fotoğraf sıralama (opsiyonel, galeri sırası için)
    /// </summary>
    /// <example>1</example>
    [Range(0, int.MaxValue, ErrorMessage = "Sıralama 0 veya pozitif bir sayı olmalıdır")]
    public int? SortOrder { get; set; }
}
