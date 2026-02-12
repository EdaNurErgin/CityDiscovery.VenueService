using VenuesService.Shared.Common.Events;

namespace CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;

public sealed class VenueApprovedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid VenueId { get; set; }
    public Guid OwnerUserId { get; set; }
    public string VenueName { get; set; } = default!;
}
