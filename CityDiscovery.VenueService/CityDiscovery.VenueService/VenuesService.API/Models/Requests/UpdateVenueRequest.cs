using System.ComponentModel.DataAnnotations;

namespace CityDiscovery.Venues.API.Models.Requests;

/// <summary>
/// Mekan güncelleme isteği
/// </summary>
/// <example>
/// {
///   "name": "Lezzet Durağı - Güncellenmiş",
///   "description": "Geleneksel Türk mutfağı ve özel lezzetler",
///   "addressText": "İstiklal Caddesi No:123, Beyoğlu, İstanbul",
///   "phone": "+905551234567",
///   "websiteUrl": "https://www.lezzetduragi.com",
///   "priceLevel": 3,
///   "openingHoursJson": "{\"Mon\":\"09:00-22:00\",\"Tue\":\"09:00-22:00\",\"Wed\":\"09:00-22:00\",\"Thu\":\"09:00-22:00\",\"Fri\":\"09:00-23:00\",\"Sat\":\"10:00-23:00\",\"Sun\":\"10:00-22:00\"}",
///   "latitude": 41.0082,
///   "longitude": 28.9784
/// }
/// </example>
public sealed class UpdateVenueRequest
{
    /// <summary>
    /// Mekan adı
    /// </summary>
    /// <example>Lezzet Durağı - Güncellenmiş</example>
    [Required(ErrorMessage = "Mekan adı zorunludur")]
    [StringLength(200, ErrorMessage = "Mekan adı en fazla 200 karakter olabilir")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Mekan açıklaması
    /// </summary>
    /// <example>Geleneksel Türk mutfağı ve özel lezzetler</example>
    [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
    public string? Description { get; set; }

    /// <summary>
    /// Tam adres
    /// </summary>
    /// <example>İstiklal Caddesi No:123, Beyoğlu, İstanbul</example>
    [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir")]
    public string? AddressText { get; set; }

    /// <summary>
    /// Telefon numarası
    /// </summary>
    /// <example>+905551234567</example>
    [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir")]
    public string? Phone { get; set; }

    /// <summary>
    /// Web sitesi URL'i
    /// </summary>
    /// <example>https://www.lezzetduragi.com</example>
    [StringLength(500, ErrorMessage = "Web sitesi URL'i en fazla 500 karakter olabilir")]
    [Url(ErrorMessage = "Geçerli bir URL formatı giriniz")]
    public string? WebsiteUrl { get; set; }

    /// <summary>
    /// Fiyat seviyesi (1-5 arası, 1=en ucuz, 5=en pahalı)
    /// </summary>
    /// <example>3</example>
    [Range(1, 5, ErrorMessage = "Fiyat seviyesi 1-5 arası olmalıdır")]
    public byte? PriceLevel { get; set; }

    /// <summary>
    /// Açılış saatleri (JSON formatında)
    /// </summary>
    /// <example>{"Mon":"09:00-22:00","Tue":"09:00-22:00","Wed":"09:00-22:00","Thu":"09:00-22:00","Fri":"09:00-23:00","Sat":"10:00-23:00","Sun":"10:00-22:00"}</example>
    public string? OpeningHoursJson { get; set; }

    /// <summary>
    /// Enlem (Latitude) - WGS84 koordinat sistemi
    /// </summary>
    /// <example>41.0082</example>
    [Range(-90, 90, ErrorMessage = "Enlem -90 ile 90 arasında olmalıdır")]
    public double Latitude { get; set; }

    /// <summary>
    /// Boylam (Longitude) - WGS84 koordinat sistemi
    /// </summary>
    /// <example>28.9784</example>
    [Range(-180, 180, ErrorMessage = "Boylam -180 ile 180 arasında olmalıdır")]
    public double Longitude { get; set; }
}
