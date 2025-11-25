using CityDiscovery.Venues.Domain.Common;

namespace CityDiscovery.Venues.Domain.Entities;

/// <summary>
/// Menüdeki ürün (ör: Latte, Cheeseburger).
/// DB: MenuItems
/// </summary>
public sealed class MenuItem : Entity, IAuditableEntity
{
    public int MenuCategoryId { get; private set; }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal? Price { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsAvailable { get; private set; }
    public int SortOrder { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public MenuCategory? MenuCategory { get; private set; }

    private MenuItem() { }

    private MenuItem(
        int menuCategoryId,
        string name,
        string? description,
        decimal? price,
        string? imageUrl,
        int sortOrder)
    {
        MenuCategoryId = menuCategoryId;
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        SortOrder = sortOrder;
        IsAvailable = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static MenuItem Create(
        int menuCategoryId,
        string name,
        string? description,
        decimal? price,
        string? imageUrl,
        int sortOrder = 0)
    {
        if (menuCategoryId <= 0)
            throw new ArgumentOutOfRangeException(nameof(menuCategoryId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (price.HasValue && price.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

        return new MenuItem(
            menuCategoryId,
            name.Trim(),
            description?.Trim(),
            price,
            imageUrl?.Trim(),
            sortOrder
        );
    }

    public void Update(
        string name,
        string? description,
        decimal? price,
        string? imageUrl,
        bool isAvailable,
        int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (price.HasValue && price.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

        Name = name.Trim();
        Description = description?.Trim();
        Price = price;
        ImageUrl = imageUrl?.Trim();
        IsAvailable = isAvailable;
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkUnavailable()
    {
        if (IsAvailable)
        {
            IsAvailable = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void MarkAvailable()
    {
        if (!IsAvailable)
        {
            IsAvailable = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
