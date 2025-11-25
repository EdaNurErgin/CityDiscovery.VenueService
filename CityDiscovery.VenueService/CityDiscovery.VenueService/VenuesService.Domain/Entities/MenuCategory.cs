using CityDiscovery.Venues.Domain.Common;

namespace CityDiscovery.Venues.Domain.Entities;

/// <summary>
/// Menüdeki kategori (ör: Kahveler, Tatlılar).
/// DB: MenuCategories
/// </summary>
public sealed class MenuCategory : Entity, IAuditableEntity
{
    public int MenuCategoryId { get; private set; } // DB'deki INT IDENTITY

    public Guid VenueId { get; private set; }
    public string Name { get; private set; } = default!;
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Venuex? Venue { get; private set; }

    private readonly List<MenuItem> _items = new();
    public IReadOnlyCollection<MenuItem> Items => _items.AsReadOnly();

    private MenuCategory() { }

    private MenuCategory(Guid venueId, string name, int sortOrder)
    {
        VenueId = venueId;
        Name = name;
        SortOrder = sortOrder;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static MenuCategory Create(Guid venueId, string name, int sortOrder = 0)
    {
        if (venueId == Guid.Empty)
            throw new ArgumentException("VenueId cannot be empty.", nameof(venueId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        return new MenuCategory(venueId, name.Trim(), sortOrder);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    internal void AddMenuItem(MenuItem item)
    {
        _items.Add(item);
    }
}
