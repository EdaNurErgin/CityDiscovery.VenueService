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
    private readonly ICityDiscoveryService _cityDiscoveryService;

    public ApproveVenueCommandHandler(
        IVenueRepository venueRepository,
        IEventPublisher eventPublisher,
        IVenueSearchService venueSearchService, ICityDiscoveryService cityDiscoveryService)
    {
        _venueRepository = venueRepository;
        _eventPublisher = eventPublisher;
        _venueSearchService = venueSearchService;
        _cityDiscoveryService = cityDiscoveryService;
    }

    public async Task<Unit> Handle(ApproveVenueCommand request, CancellationToken cancellationToken)
    {
        // 1. Mekanı bul
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue is null)
            throw new KeyNotFoundException($"Venue not found with id: {request.VenueId}");

        // 2. Onayla ve Veritabanını Güncelle
        venue.Approve();
        await _venueRepository.UpdateAsync(venue, cancellationToken);

        
        //Nesneyi Tüm İlişkileriyle Yeniden Yükle
        // Update sonrası navigation property'lerin null kalma riskine karşı 
        // mekanı tekrar 'Include'lar ile tertemiz çekiyoruz.
        
        var updatedVenue = await _venueRepository.GetVenueWithDetailsAsync(request.VenueId, cancellationToken);
        //  ELASTICSEARCH'E KAYDET (Eski ve Yeni Yapı)
        try
        {
            // Eski arama için temel DTO
            var venueBasicDto = new VenueBasicDto
            {
                Id = updatedVenue.Id,
                Name = updatedVenue.Name,
                PriceLevel = updatedVenue.PriceLevel?.Value,
                Location = new LocationDto
                {
                    Latitude = updatedVenue.Location?.Y ?? 0,
                    Longitude = updatedVenue.Location?.X ?? 0
                }
            };
            await _venueSearchService.IndexVenueAsync(venueBasicDto);

            // Yeni filtreli arama için ZENGİN model
            // updatedVenue kullandığımız için Address.City.Name artık dolu gelecek!
            await _cityDiscoveryService.IndexVenueWithDetailsAsync(updatedVenue);
        }
        catch (Exception ex)
        {
            // Loglama yapabilirsiniz
        }

        // 5. Event yayınla
        try
        {
            await _eventPublisher.PublishAsync(new VenueApprovedEvent
            {
                VenueId = updatedVenue.Id,
                OwnerUserId = updatedVenue.OwnerUserId,
                VenueName = updatedVenue.Name,
                ApprovedAt = DateTime.UtcNow
            }, cancellationToken);
        }
        catch (Exception) { }

        return Unit.Value;
    }

}