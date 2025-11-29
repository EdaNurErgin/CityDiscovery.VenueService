using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Shared.Common.Events.Identity;
using MassTransit;

namespace CityDiscovery.Venues.Infrastructure.MessageBus.Consumers;

/// <summary>
/// Identity Service'den UserDeletedEvent geldiğinde,
/// eğer bu user Owner ise mekanını deaktif et.
/// </summary>
public sealed class UserDeletedEventConsumer : IConsumer<UserDeletedEvent>
{
    private readonly IVenueRepository _venueRepository;

    public UserDeletedEventConsumer(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task Consume(ConsumeContext<UserDeletedEvent> context)
    {
        var userId = context.Message.UserId;

        var venue = await _venueRepository.GetByOwnerIdAsync(userId, context.CancellationToken);

        if (venue is not null)
        {
            venue.Deactivate();
            await _venueRepository.UpdateAsync(venue, context.CancellationToken);
        }
    }
}