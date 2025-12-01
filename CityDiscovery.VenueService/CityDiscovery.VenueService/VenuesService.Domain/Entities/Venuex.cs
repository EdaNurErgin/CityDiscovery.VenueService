using CityDiscovery.Venues.Domain.Common;
using CityDiscovery.Venues.Domain.ValueObjects;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace CityDiscovery.Venues.Domain.Entities;

/// <summary>
/// Mekan (Venue) aggregate root'u.
/// Venues tablosuna karşılık gelir.
/// </summary>
public sealed class Venuex : AggregateRoot, IAuditableEntity
{
    // SRID 4326 için tek bir geometry factory
    private static readonly GeometryFactory GeometryFactory =
        NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

    public Guid OwnerUserId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? AddressText { get; private set; }
    public string? Phone { get; private set; }
    public string? WebsiteUrl { get; private set; }

    public PriceLevel? PriceLevel { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    /// <summary>
    /// Açılış saatleri JSON string olarak tutulacak.
    /// Örn: {"Mon":"09:00-22:00", "Tue":"09:00-22:00", ...}
    /// </summary>
    public string? OpeningHoursJson { get; private set; }

    /// <summary>
    /// WGS84 (SRID 4326) konum.
    /// NetTopologySuite Point olarak tutulur (X = Longitude, Y = Latitude)
    /// </summary>
    public Point Location { get; private set; } = default!;

    public bool IsApproved { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // 1:1 adres
    public VenueAddress? Address { get; private set; }

    // Venue ↔ Category (M:N, join tablo: VenueCategory)
    private readonly List<VenueCategory> _venueCategories = new();
    public IReadOnlyCollection<VenueCategory> VenueCategories => _venueCategories.AsReadOnly();

    // Fotograflar
    private readonly List<VenuePhoto> _photos = new();
    public IReadOnlyCollection<VenuePhoto> Photos => _photos.AsReadOnly();

    // Menü kategorileri
    private readonly List<MenuCategory> _menuCategories = new();
    public IReadOnlyCollection<MenuCategory> MenuCategories => _menuCategories.AsReadOnly();

    // Etkinlikler
    private readonly List<Event> _events = new();
    public IReadOnlyCollection<Event> Events => _events.AsReadOnly();

    private Venuex() { } // EF için

    private Venuex(
        Guid ownerUserId,
        string name,
        string? description,
        string? addressText,
        string? phone,
        string? websiteUrl,
        PriceLevel? priceLevel,
        string? openingHoursJson,
        Point location)
    {
        OwnerUserId = ownerUserId;
        Name = name;
        Description = description;
        AddressText = addressText;
        Phone = phone;
        WebsiteUrl = websiteUrl;
        PriceLevel = priceLevel;
        OpeningHoursJson = openingHoursJson;
        Location = location;

        IsApproved = false;
        IsActive = true;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public static Venuex Create(
        Guid ownerUserId,
        string name,
        string? description,
        string? addressText,
        string? phone,
        string? websiteUrl,
        PriceLevel? priceLevel,
        string? openingHoursJson,
        double latitude,
        double longitude)
    {
        if (ownerUserId == Guid.Empty)
            throw new ArgumentException("OwnerUserId cannot be empty.", nameof(ownerUserId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        // NTS: X = lon, Y = lat
        var point = GeometryFactory.CreatePoint(new Coordinate(longitude, latitude));

        return new Venuex(
            ownerUserId,
            name.Trim(),
            description?.Trim(),
            addressText?.Trim(),
            phone?.Trim(),
            websiteUrl?.Trim(),
            priceLevel,
            openingHoursJson,
            point
        );
    }



    #region Behavior

    public void UpdateBasicInfo(
        string name,
        string? description,
        string? addressText,
        string? phone,
        string? websiteUrl,
        PriceLevel? priceLevel,
        string? openingHoursJson,
        double latitude,
        double longitude)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        AddressText = addressText?.Trim();
        Phone = phone?.Trim();
        WebsiteUrl = websiteUrl?.Trim();
        PriceLevel = priceLevel;
        OpeningHoursJson = openingHoursJson;

        // Yeni lokasyon
        Location = GeometryFactory.CreatePoint(new Coordinate(longitude, latitude));

        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        if (!IsApproved)
        {
            IsApproved = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Reject()
    {
        if (IsApproved)
        {
            IsApproved = false;
            UpdatedAt = DateTime.UtcNow;
        }
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

    public void SetAddress(VenueAddress address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
        UpdatedAt = DateTime.UtcNow;
    }

    // Profil fotoğrafını güncellemek için domain metodu
    public void UpdateProfilePicture(string? pictureUrl)
    {
        ProfilePictureUrl = pictureUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion

    #region Collections Helpers

    public void AddCategory(VenueCategory category)
    {
        _venueCategories.Add(category);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPhoto(VenuePhoto photo)
    {
        _photos.Add(photo);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMenuCategory(MenuCategory category)
    {
        _menuCategories.Add(category);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddEvent(Event @event)
    {
        _events.Add(@event);
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion
}
