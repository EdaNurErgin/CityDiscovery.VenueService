using VenuesService.Shared.Common.Events;

namespace CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;

/// <summary>
/// Yeni mekan oluşturulduğunda yayınlanır.
/// → Admin Service: "Yeni mekan onay bekliyor" bildirimi
/// </summary>
public sealed class VenueCreatedEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid VenueId { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Name { get; set; } = default!;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}