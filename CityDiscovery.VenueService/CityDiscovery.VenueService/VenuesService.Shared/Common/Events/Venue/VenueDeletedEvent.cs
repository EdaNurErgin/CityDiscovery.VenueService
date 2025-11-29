using VenuesService.Shared.Common.Events;

namespace CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;

/// <summary>
/// Mekan silindiğinde yayınlanır.
/// → Social Service: Bu mekana ait postları soft delete
/// → Review Service: Bu mekana ait yorumları soft delete
/// </summary>
public sealed class VenueDeletedEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid VenueId { get; set; }
    public string VenueName { get; set; } = default!;
    public DateTime DeletedAt { get; set; }
}