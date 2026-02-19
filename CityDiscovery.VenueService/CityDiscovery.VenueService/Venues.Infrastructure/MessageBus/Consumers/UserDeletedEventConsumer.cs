using MassTransit;
using Microsoft.Extensions.Logging;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using IdentityService.Shared.MessageBus.Identity;

namespace CityDiscovery.Venues.Infrastructure.MessageBus.Consumers;

public sealed class UserDeletedEventConsumer : IConsumer<UserDeletedEvent>
{
    private readonly IVenueRepository _venueRepository;
    private readonly ILogger<UserDeletedEventConsumer> _logger;

    public UserDeletedEventConsumer(IVenueRepository venueRepository, ILogger<UserDeletedEventConsumer> logger)
    {
        _venueRepository = venueRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserDeletedEvent> context)
    {
        var userId = context.Message.UserId;
        _logger.LogInformation("UserDeletedEvent received for User ID: {UserId}. Deactivating owned venues...", userId);

        try
        {
            
            // Tek tek çekip update etmek yerine, Repository'deki toplu silme metodunu çağırıyoruz.
            // Bu metot kullanıcının 1 tane de olsa 10 tane de olsa mekanını bulup kapatır.
            await _venueRepository.DeactivateVenuesByOwnerAsync(userId);

            _logger.LogInformation("Deactivation process completed for Owner {UserId}.", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deactivating venues for User ID: {UserId}", userId);
            // Hata durumunda kuyruğa geri bırakmak istersen:
            // throw; 
        }
    }
}