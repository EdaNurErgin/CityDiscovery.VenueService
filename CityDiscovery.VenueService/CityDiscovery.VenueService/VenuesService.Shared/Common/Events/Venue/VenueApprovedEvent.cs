using VenuesService.Shared.Common.Events;

namespace CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;

/// <summary>
/// Mekan onaylandığında yayınlanır.
/// → Notification Service: Owner'a "Mekanınız onaylandı" bildirimi
/// </summary>
public sealed class VenueApprovedEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid VenueId { get; set; }
    public Guid OwnerUserId { get; set; }
    public string VenueName { get; set; } = default!;
    public DateTime ApprovedAt { get; set; }
}