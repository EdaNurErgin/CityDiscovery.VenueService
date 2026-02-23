using CityDiscovery.Venues.Application.Interfaces.MessageBus;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;
using CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;
using MediatR;


namespace CityDiscovery.Venues.Application.Features.Venues.Commands.ApproveVenue;

public sealed class ApproveVenueCommandHandler : IRequestHandler<ApproveVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IVenueSearchService _venueSearchService;

    public ApproveVenueCommandHandler(
        IVenueRepository venueRepository,
        IEventPublisher eventPublisher,
        IVenueSearchService venueSearchService) 
    {
        _venueRepository = venueRepository;
        _eventPublisher = eventPublisher;
        _venueSearchService = venueSearchService; 
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

        //  ONAYLANDIKTAN SONRA ELASTICSEARCH'E KAYDET ---
        try
        {
            var venueBasicDto = new VenueBasicDto
            {
                Id = venue.Id,
                Name = venue.Name,
                AddressText = venue.AddressText,
                PriceLevel = venue.PriceLevel?.Value, // PriceLevel bir ValueObject ise değerini alıyoruz
                Location = new LocationDto
                {
                    Latitude = venue.Location?.Y ?? 0,   // Y ekseni Latitude'dur
                    Longitude = venue.Location?.X ?? 0   // X ekseni Longitude'dur
                }
            };

            await _venueSearchService.IndexVenueAsync(venueBasicDto);
        }
        catch (Exception)
        {
            // Elastic kayıt hatası olsa bile mekan onaylanmış olmalı, hata fırlatmadan calisir
        }
        

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