using System.ComponentModel.DataAnnotations;

namespace CityDiscovery.VenueService.VenuesService.API.Models.Requests;

/// <summary>
/// Etkinlik oluşturma isteği
/// </summary>
/// <example>
/// {
///   "title": "Canlı Müzik Gecesi",
///   "description": "Her Cuma akşamı canlı müzik",
///   "startDate": "2025-12-01T20:00:00Z",
///   "endDate": "2025-12-01T23:00:00Z",
///   "imageUrl": "https://example.com/photos/live-music.jpg"
/// }
/// </example>
public sealed class CreateEventRequest
{
    /// <summary>
    /// Etkinlik başlığı
    /// </summary>
    /// <example>Canlı Müzik Gecesi</example>
    [Required(ErrorMessage = "Etkinlik başlığı zorunludur")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
    public string Title { get; set; } = default!;

    /// <summary>
    /// Etkinlik açıklaması (opsiyonel)
    /// </summary>
    /// <example>Her Cuma akşamı canlı müzik</example>
    [StringLength(2000, ErrorMessage = "Açıklama en fazla 2000 karakter olabilir")]
    public string? Description { get; set; }

    /// <summary>
    /// Etkinlik başlangıç tarihi
    /// </summary>
    /// <example>2025-12-01T20:00:00Z</example>
    [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Etkinlik bitiş tarihi (opsiyonel)
    /// </summary>
    /// <example>2025-12-01T23:00:00Z</example>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Etkinlik görsel URL'i (opsiyonel)
    /// </summary>
    /// <example>https://example.com/photos/live-music.jpg</example>
    [Url(ErrorMessage = "Geçerli bir URL formatı giriniz")]
    public string? ImageUrl { get; set; }
}
