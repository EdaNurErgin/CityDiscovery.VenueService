using CityDiscovery.Venues.Shared.Common.Events;
using VenuesService.Shared.Common.Events;

namespace CityDiscovery.Venues.Application.Interfaces.MessageBus;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}