using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.ValueObjects;
using CityDiscovery.Venues.Application.Interfaces.MessageBus;
using CityDiscovery.VenuesService.Shared.Common.Events.Venue;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateVenueBasicInfo;

public sealed class UpdateVenueBasicInfoCommandHandler
    : IRequestHandler<UpdateVenueBasicInfoCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IEventPublisher _eventPublisher;

    public UpdateVenueBasicInfoCommandHandler(
        IVenueRepository venueRepository,
        IEventPublisher eventPublisher)
    {
        _venueRepository = venueRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Unit> Handle(UpdateVenueBasicInfoCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue not found with id: {request.VenueId}");

        var priceLevel = request.PriceLevel.HasValue
            ? PriceLevel.Create(request.PriceLevel.Value)
            : null;

        venue.UpdateBasicInfo(
            request.Name,
            request.Description,
            request.AddressText,
            request.Phone,
            request.WebsiteUrl,
            priceLevel,
            request.OpeningHoursJson,
            request.Latitude,
            request.Longitude
        );

        await _venueRepository.UpdateAsync(venue, cancellationToken);

        // --- DÜZELTİLEN KISIM ---

        // HATA BURADAYDI: venue.Photos içinde IsProfilePicture aramaya gerek yok.
        // Entity'nizde 'ProfilePictureUrl' alanı zaten ana tabloda tutuluyor.

        var integrationEvent = new VenueUpdatedEvent
        {
            VenueId = venue.Id,
            Name = venue.Name,
            Description = venue.Description ?? string.Empty, // Null gelirse boş string gönder
            ImageUrl = venue.ProfilePictureUrl ?? string.Empty // Direkt entity'den alıyoruz
        };

        await _eventPublisher.PublishAsync(integrationEvent, cancellationToken);
        // -------------------------

        return Unit.Value;
    }
}