using System.ComponentModel.DataAnnotations;

namespace CityDiscovery.Venues.API.Models.Requests;

public sealed class SetVenueAddressRequest
{
    [Required(ErrorMessage = "Ülke seçimi zorunludur.")]
    public int CountryId { get; set; }

    [Required(ErrorMessage = "Şehir seçimi zorunludur.")]
    public int CityId { get; set; }

    [Required(ErrorMessage = "İlçe seçimi zorunludur.")]
    public int DistrictId { get; set; }

    // YENİ EKLENEN MANUEL ALANLAR
    [StringLength(200, ErrorMessage = "Mahalle adı en fazla 200 karakter olabilir.")]
    public string? Neighborhood { get; set; }

    [StringLength(200, ErrorMessage = "Sokak/Cadde adı en fazla 200 karakter olabilir.")]
    public string? Street { get; set; }

    [StringLength(50, ErrorMessage = "Bina/Kapı no en fazla 50 karakter olabilir.")]
    public string? BuildingNo { get; set; }

    [StringLength(500, ErrorMessage = "Açık adres en fazla 500 karakter olabilir.")]
    public string? FullAddress { get; set; }
}