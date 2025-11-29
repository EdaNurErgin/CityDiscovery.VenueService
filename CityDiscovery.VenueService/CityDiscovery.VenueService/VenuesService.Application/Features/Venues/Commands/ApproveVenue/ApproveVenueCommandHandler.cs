//using CityDiscovery.Venues.Application.Interfaces.Repositories;
//using MediatR;

//namespace CityDiscovery.Venues.Application.Features.Venues.Commands.ApproveVenue;

//public sealed class ApproveVenueCommandHandler: IRequestHandler<ApproveVenueCommand, Unit>
//{
//    private readonly IVenueRepository _venueRepository;

//    public ApproveVenueCommandHandler(IVenueRepository venueRepository)
//    {
//        _venueRepository = venueRepository;
//    }

//    public async Task<Unit> Handle(ApproveVenueCommand request, CancellationToken cancellationToken)
//    {
//        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

//        if (venue is null)
//            throw new KeyNotFoundException($"Venue not found with id: {request.VenueId}");

//        // Domain davranışı: Onayla
//        venue.Approve();

//        await _venueRepository.UpdateAsync(venue, cancellationToken);

//        return Unit.Value;
//    }
//}

using CityDiscovery.Venues.Application.Interfaces.MessageBus;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.ApproveVenue;

public sealed class ApproveVenueCommandHandler : IRequestHandler<ApproveVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IEventPublisher _eventPublisher;

    public ApproveVenueCommandHandler(
        IVenueRepository venueRepository,
        IEventPublisher eventPublisher)
    {
        _venueRepository = venueRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Unit> Handle(
        ApproveVenueCommand request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue not found with id: {request.VenueId}");

        // Domain davranışı: Onayla
        venue.Approve();

        await _venueRepository.UpdateAsync(venue, cancellationToken);

        // Event yayınla (Notification Service'e bildirim gitmesi için)
        // Event yayınlama hatası olsa bile venue onaylanmış olmalı
        try
        {
            await _eventPublisher.PublishAsync(new VenueApprovedEvent
            {
                VenueId = venue.Id,
                OwnerUserId = venue.OwnerUserId,
                VenueName = venue.Name,
                ApprovedAt = DateTime.UtcNow
            }, cancellationToken);
        }
        catch (Exception)
        {
            // Event yayınlama hatası olsa bile venue onaylanmış, log'la ama exception fırlatma
        }

        return Unit.Value;
    }
}
