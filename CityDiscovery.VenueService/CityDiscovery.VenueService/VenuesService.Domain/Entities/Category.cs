using CityDiscovery.Venues.Domain.Common;

namespace CityDiscovery.Venues.Domain.Entities;

/// <summary>
/// Mekan kategorisi (örn: Cafe, Restaurant, Museum).
/// DB'de Categories tablosuna karşılık gelir.
/// </summary>
public sealed class Category : Entity
{
    // INT IDENTITY ama Domain tarafında Guid Id var; EF ile override edeceğiz istersen.
    // İstersen burada int Id de kullanabiliriz; ama base Entity Guid. 
    // En basiti: Category için ayrı bir base kullanmak yerine, sadece int Id property ekleyebiliriz.
    // Ben burada int Id'li versiyonu seçiyorum, çünkü tablo INT IDENTITY.

    public int CategoryId { get; private set; }  // DB'deki Id kolonu

    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? IconUrl { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<VenueCategory> VenueCategories => _venueCategories.AsReadOnly();
    private readonly List<VenueCategory> _venueCategories = new();

    private Category() { }

    private Category(string name, string slug, string? iconUrl)
    {
        Name = name;
        Slug = slug;
        IconUrl = iconUrl;
        IsActive = true;
    }

    public static Category Create(string name, string slug, string? iconUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug is required.", nameof(slug));

        return new Category(name.Trim(), slug.Trim().ToLowerInvariant(), iconUrl?.Trim());
    }

    public void Update(string name, string slug, string? iconUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug is required.", nameof(slug));

        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        IconUrl = iconUrl?.Trim();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
