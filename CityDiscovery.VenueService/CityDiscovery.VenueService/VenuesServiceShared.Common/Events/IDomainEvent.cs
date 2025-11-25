namespace CityDiscovery.Venues.Shared.Common.Events;

/// <summary>
/// Domain içinde kullanılan event'ler için marker interface.
/// Bu event'ler domain katmanında üretilir, dışarıya direkt gitmez.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
