using CityDiscovery.Venues.Shared.Common.Events;
using VenuesService.Shared.Common.Events;

namespace CityDiscovery.Venues.Shared.Common.Events.Identity;

/// <summary>
/// Identity Service'den yayınlanır.
/// VenueService bu event'i dinler ve owner silinmişse mekanı deaktif eder.
/// </summary>
public sealed class UserDeletedEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
    public DateTime OccurredOn { get; set; }

    public Guid UserId { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime DeletedAt { get; set; }
}