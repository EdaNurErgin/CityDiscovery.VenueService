using CityDiscovery.Venues.Domain.Common;

namespace CityDiscovery.Venues.Domain.Entities;

/// <summary>
/// Venue'nin detaylı adres bilgileri. DB'de VenueAddresses tablosuna karşılık gelir.
/// 1:1 ilişki (Venue başına 1 adres).
/// </summary>
public sealed class VenueAddress : Entity
{
    public Guid VenueId { get; private set; }

    public int CountryId { get; private set; }
    public int CityId { get; private set; }
    public int DistrictId { get; private set; }

    public string? Neighborhood { get; private set; }
    public string? Street { get; private set; }
    public string? BuildingNo { get; private set; }
    public string? FullAddress { get; private set; }

    private VenueAddress() { } // EF için

    private VenueAddress(
        Guid venueId,
        int countryId,
        int cityId,
        int districtId,
        string? neighborhood,
        string? street,
        string? buildingNo,
        string? fullAddress)
    {
        VenueId = venueId;
        CountryId = countryId;
        CityId = cityId;
        DistrictId = districtId;
        Neighborhood = neighborhood;
        Street = street;
        BuildingNo = buildingNo;
        FullAddress = fullAddress;
    }

    public static VenueAddress Create(
        Guid venueId,
        int countryId,
        int cityId,
        int districtId,
        string? neighborhood,
        string? street,
        string? buildingNo,
        string? fullAddress)
    {
        if (venueId == Guid.Empty)
            throw new ArgumentException("VenueId cannot be empty.", nameof(venueId));

        if (countryId <= 0) throw new ArgumentOutOfRangeException(nameof(countryId));
        if (cityId <= 0) throw new ArgumentOutOfRangeException(nameof(cityId));
        if (districtId <= 0) throw new ArgumentOutOfRangeException(nameof(districtId));

        return new VenueAddress(
            venueId,
            countryId,
            cityId,
            districtId,
            neighborhood,
            street,
            buildingNo,
            fullAddress
        );
    }

    public void Update(
        int countryId,
        int cityId,
        int districtId,
        string? neighborhood,
        string? street,
        string? buildingNo,
        string? fullAddress)
    {
        CountryId = countryId;
        CityId = cityId;
        DistrictId = districtId;
        Neighborhood = neighborhood;
        Street = street;
        BuildingNo = buildingNo;
        FullAddress = fullAddress;
    }
}
