using CityDiscovery.Venues.Domain.Common;

namespace CityDiscovery.Venues.Domain.Entities;

public sealed class District : Entity
{
    public int DistrictId { get; private set; } // PK
    public int CityId { get; private set; } // FK
    public string Name { get; private set; } = default!;

    // Navigation
    public City? City { get; private set; }

    private District() { }

    public static District Create(int cityId, string name)
    {
        if (cityId <= 0) throw new ArgumentOutOfRangeException(nameof(cityId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("District name is required.", nameof(name));

        return new District
        {
            CityId = cityId,
            Name = name.Trim()
        };
    }
}