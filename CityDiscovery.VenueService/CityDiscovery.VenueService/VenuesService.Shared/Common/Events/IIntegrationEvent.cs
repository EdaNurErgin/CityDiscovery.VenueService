namespace VenuesService.Shared.Common.Events;

/// <summary>
/// Mikroservisler arası iletişim (RabbitMQ, MassTransit vs.) için kullanılan event'ler.
/// Bunlar Shared.MessageBus üzerinden publish edilecek.
/// </summary>
public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}
