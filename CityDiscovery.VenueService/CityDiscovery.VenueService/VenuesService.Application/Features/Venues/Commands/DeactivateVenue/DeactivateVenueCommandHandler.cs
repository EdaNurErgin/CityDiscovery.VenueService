using CityDiscovery.Venues.Application.Interfaces.MessageBus;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.DeactivateVenue;

public sealed class DeactivateVenueCommandHandler
    : IRequestHandler<DeactivateVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IEventPublisher _eventPublisher;

    public DeactivateVenueCommandHandler(
        IVenueRepository venueRepository,
        IEventPublisher eventPublisher)
    {
        _venueRepository = venueRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Unit> Handle(DeactivateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue not found with id: {request.VenueId}");

        // Domain davranışı
        venue.Deactivate();

        await _venueRepository.UpdateAsync(venue, cancellationToken);

        // Event yayınla (Social ve Review servislerine bildirim için)
        // Event yayınlama hatası olsa bile venue deaktif edilmiş olmalı
        try
        {
            await _eventPublisher.PublishAsync(new VenueDeactivatedEvent
            {
                VenueId = venue.Id,
                OwnerUserId = venue.OwnerUserId,
                DeactivatedAt = DateTime.UtcNow
            }, cancellationToken);
        }
        catch (Exception)
        {
            // Event yayınlama hatası olsa bile venue deaktif edilmiş, log'la ama exception fırlatma
            // Çünkü veri tutarlılığı önemli, event sonra tekrar gönderilebilir
        }

        return Unit.Value;
    }
}
