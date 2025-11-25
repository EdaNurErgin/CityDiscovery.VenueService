using CityDiscovery.Venues.Domain.Common;

namespace CityDiscovery.Venues.Domain.Entities;

/// <summary>
/// Venue - Category M:N ilişki tablosu (VenueCategories).
/// </summary>
public sealed class VenueCategory : Entity
{
    public Guid VenueId { get; private set; }
    public int CategoryId { get; private set; }

    // Navigation
    public Venuex? Venue { get; private set; }
    public Category? Category { get; private set; }

    private VenueCategory() { }

    private VenueCategory(Guid venueId, int categoryId)
    {
        VenueId = venueId;
        CategoryId = categoryId;
    }

    public static VenueCategory Create(Guid venueId, int categoryId)
    {
        if (venueId == Guid.Empty)
            throw new ArgumentException("VenueId cannot be empty.", nameof(venueId));

        if (categoryId <= 0)
            throw new ArgumentOutOfRangeException(nameof(categoryId));

        return new VenueCategory(venueId, categoryId);
    }
}
