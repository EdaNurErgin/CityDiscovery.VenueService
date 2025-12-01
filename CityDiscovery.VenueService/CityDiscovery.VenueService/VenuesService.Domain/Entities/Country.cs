using CityDiscovery.Venues.Domain.Common;

namespace CityDiscovery.Venues.Domain.Entities;

public sealed class Country : Entity
{
    public int CountryId { get; private set; } // PK
    public string Name { get; private set; } = default!;
    public string? Code { get; private set; } //  TR, US

    // Navigation
    private readonly List<City> _cities = new();
    public IReadOnlyCollection<City> Cities => _cities.AsReadOnly();

    private Country() { }

    public static Country Create(string name, string? code)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Country name is required.", nameof(name));

        return new Country
        {
            Name = name.Trim(),
            Code = code?.Trim().ToUpperInvariant()
        };
    }
}