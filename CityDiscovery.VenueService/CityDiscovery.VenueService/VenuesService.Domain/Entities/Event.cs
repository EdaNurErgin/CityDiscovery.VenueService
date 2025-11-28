using CityDiscovery.Venues.Domain.Common;

namespace CityDiscovery.Venues.Domain.Entities;

public sealed class Event : Entity, IAuditableEntity
{
    public Guid VenueId { get; private set; }

    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Venuex? Venue { get; private set; }

    private Event() { }

    private Event(
        Guid venueId,
        string title,
        string? description,
        DateTime startDate,
        DateTime? endDate,
        string? imageUrl)
    {
        VenueId = venueId;
        Title = title;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        ImageUrl = imageUrl;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static Event Create(
        Guid venueId,
        string title,
        string? description,
        DateTime startDate,
        DateTime? endDate,
        string? imageUrl)
    {
        if (venueId == Guid.Empty)
            throw new ArgumentException("VenueId cannot be empty.", nameof(venueId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (endDate.HasValue && endDate.Value < startDate)
            throw new ArgumentException("EndDate cannot be earlier than StartDate.", nameof(endDate));

        return new Event(
            venueId,
            title.Trim(),
            description?.Trim(),
            startDate,
            endDate,
            imageUrl?.Trim());
    }

    public void Update(
        string title,
        string? description,
        DateTime startDate,
        DateTime? endDate,
        string? imageUrl,
        bool isActive)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (endDate.HasValue && endDate.Value < startDate)
            throw new ArgumentException("EndDate cannot be earlier than StartDate.", nameof(endDate));

        Title = title.Trim();
        Description = description?.Trim();
        StartDate = startDate;
        EndDate = endDate;
        ImageUrl = imageUrl?.Trim();
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateImage(string? imageUrl)
    {
        ImageUrl = imageUrl?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

}
