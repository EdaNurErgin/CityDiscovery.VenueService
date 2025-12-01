using CityDiscovery.Venues.Domain.Common;

namespace CityDiscovery.Venues.Domain.Entities;

public sealed class City : Entity
{
    public int CityId { get; private set; } // PK
    public int CountryId { get; private set; } // FK
    public string Name { get; private set; } = default!;

    // Navigation
    public Country? Country { get; private set; }

    private readonly List<District> _districts = new();
    public IReadOnlyCollection<District> Districts => _districts.AsReadOnly();

    private City() { }

    public static City Create(int countryId, string name)
    {
        if (countryId <= 0) throw new ArgumentOutOfRangeException(nameof(countryId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("City name is required.", nameof(name));

        return new City
        {
            CountryId = countryId,
            Name = name.Trim()
        };
    }
}