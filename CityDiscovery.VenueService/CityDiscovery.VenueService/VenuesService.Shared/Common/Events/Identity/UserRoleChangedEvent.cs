using CityDiscovery.Venues.Shared.Common.Events;
using VenuesService.Shared.Common.Events;

namespace CityDiscovery.Venues.Shared.Common.Events.Identity;

/// <summary>
/// Identity Service'den yayınlanır.
/// VenueService bu event'i dinleyip log tutabilir.
/// </summary>
public sealed class UserRoleChangedEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
    public DateTime OccurredOn { get; set; }

    public Guid UserId { get; set; }
    public string OldRole { get; set; } = default!;
    public string NewRole { get; set; } = default!;
}