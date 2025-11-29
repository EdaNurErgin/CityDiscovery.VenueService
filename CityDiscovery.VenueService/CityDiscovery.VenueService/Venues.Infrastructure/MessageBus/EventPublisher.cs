using CityDiscovery.Venues.Application.Interfaces.MessageBus;
using CityDiscovery.Venues.Shared.Common.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using VenuesService.Shared.Common.Events;

namespace CityDiscovery.Venues.Infrastructure.MessageBus;

public sealed class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<EventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        try
        {
            // Timeout için CancellationTokenSource kullan
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(5)); // 5 saniye timeout

            await _publishEndpoint.Publish(@event, timeoutCts.Token);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Event publishing timeout: {EventType}", typeof(TEvent).Name);
            // Timeout olsa bile exception fırlatma, sadece log'la
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event: {EventType}", typeof(TEvent).Name);
            // RabbitMQ bağlantı hatası olsa bile exception fırlatma, sadece log'la
            // Event sonra tekrar gönderilebilir (outbox pattern ile)
        }
    }
}