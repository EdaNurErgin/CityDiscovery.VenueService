using CityDiscovery.Venues.Domain.Common;

namespace CityDiscovery.Venues.Domain.Entities;

/// <summary>
/// Mekan fotoğrafları. DB: VenuePhotos
/// </summary>
public sealed class VenuePhoto : Entity, IAuditableEntity
{
    public Guid VenueId { get; private set; }
    public string Url { get; private set; } = default!;
    public string? Caption { get; private set; }
    public int SortOrder { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt => null; // tabloda yok ama interface gereği

    // Navigation
    public Venuex? Venue { get; private set; }

    private VenuePhoto() { }

    private VenuePhoto(Guid venueId, string url, string? caption, int sortOrder)
    {
        VenueId = venueId;
        Url = url;
        Caption = caption;
        SortOrder = sortOrder;
        CreatedAt = DateTime.UtcNow;
    }

    public static VenuePhoto Create(Guid venueId, string url, string? caption, int sortOrder = 0)
    {
        if (venueId == Guid.Empty)
            throw new ArgumentException("VenueId cannot be empty.", nameof(venueId));

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Url is required.", nameof(url));

        return new VenuePhoto(venueId, url.Trim(), caption?.Trim(), sortOrder);
    }

    public void UpdateCaption(string? caption)
    {
        Caption = caption?.Trim();
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }
}
