using VenuesService.Shared.Common.Events;

namespace CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;

/// <summary>
/// Mekan deaktif edildiğinde yayınlanır.
/// → Social Service: Post'ları gizle
/// → Review Service: Yorumları gizle
/// </summary>
public sealed class VenueDeactivatedEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid VenueId { get; set; }
    public Guid OwnerUserId { get; set; }
    public DateTime DeactivatedAt { get; set; }
}