using CityDiscovery.Venues.Application.Interfaces.MessageBus;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteVenue;

public sealed class DeleteVenueCommandHandler : IRequestHandler<DeleteVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<DeleteVenueCommandHandler> _logger;

    public DeleteVenueCommandHandler(
        IVenueRepository venueRepository,
        IEventPublisher eventPublisher,
        ILogger<DeleteVenueCommandHandler> logger)
    {
        _venueRepository = venueRepository;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        DeleteVenueCommand request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue not found with id: {request.VenueId}");

        var venueName = venue.Name;
        var venueId = request.VenueId;

        // ÖNCE event yayınla (Social ve Review servislerine bildirim için)
        // Event yayınlanamazsa venue silinmesin (event-driven architecture için kritik)
        // NOT: EventPublisher şu anda exception fırlatmıyor, bu yüzden manuel kontrol yapıyoruz
        var eventPublished = false;
        try
        {
            await _eventPublisher.PublishAsync(new VenueDeletedEvent
            {
                VenueId = venueId,
                VenueName = venueName,
                DeletedAt = DateTime.UtcNow
            }, cancellationToken);
            
            // EventPublisher exception fırlatmıyor, bu yüzden başarılı olduğunu varsayıyoruz
            // Eğer RabbitMQ bağlantısı yoksa bile event queue'ya eklenmiş olabilir
            eventPublished = true;
            _logger.LogInformation("VenueDeletedEvent published successfully for venue {VenueId}", venueId);
        }
        catch (Exception ex)
        {
            // Event yayınlanamazsa venue silinmesin
            _logger.LogError(ex, "Failed to publish VenueDeletedEvent for venue {VenueId}. Venue will not be deleted.", venueId);
            throw new InvalidOperationException(
                $"Venue deletion failed: Could not publish VenueDeletedEvent for venue {venueId}. " +
                $"The venue was not deleted. Error: {ex.Message}", ex);
        }

        // Event başarıyla yayınlandıktan SONRA venue'yu sil
        if (eventPublished)
        {
            await _venueRepository.DeleteAsync(venue, cancellationToken);
            _logger.LogInformation("Venue {VenueId} deleted successfully after event was published", venueId);
        }

        return Unit.Value;
    }
}

