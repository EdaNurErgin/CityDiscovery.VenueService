using CityDiscovery.Venues.Application.Interfaces.MessageBus;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.ValueObjects;
using CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;
using CityDiscovery.VenuesService.Shared.Common.Events.Venue;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateVenueBasicInfo;

public sealed class UpdateVenueBasicInfoCommandHandler
    : IRequestHandler<UpdateVenueBasicInfoCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICityDiscoveryService _cityDiscoveryService; // 2. ES İÇİN EKLENDİ
    private readonly IVenueSearchService _venueSearchService;
    public UpdateVenueBasicInfoCommandHandler(
        IVenueRepository venueRepository,
        IEventPublisher eventPublisher, ICityDiscoveryService cityDiscoveryService,
        IVenueSearchService venueSearchService)
    {
        _venueRepository = venueRepository;
        _eventPublisher = eventPublisher;
        _cityDiscoveryService = cityDiscoveryService;
        _venueSearchService = venueSearchService;
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
            request.Phone,
            request.WebsiteUrl,
            priceLevel,
            request.OpeningHoursJson,
            request.Latitude,
            request.Longitude
        );

        await _venueRepository.UpdateAsync(venue, cancellationToken);

        var venueForElastic = await _venueRepository.GetVenueWithDetailsAsync(request.VenueId, cancellationToken);

        if (venueForElastic != null && venueForElastic.IsApproved)
        {
            
            var venueBasicDto = new VenueBasicDto
            {
                Id = venueForElastic.Id,
                Name = venueForElastic.Name,
                PriceLevel = venueForElastic.PriceLevel?.Value,
                Location = new LocationDto
                {
                    Latitude = venueForElastic.Location?.Y ?? 0,
                    Longitude = venueForElastic.Location?.X ?? 0
                }
            };
            await _venueSearchService.IndexVenueAsync(venueBasicDto);

            
            await _cityDiscoveryService.IndexVenueWithDetailsAsync(venueForElastic);
        }

        var integrationEvent = new VenueUpdatedEvent
        {
            VenueId = venue.Id,
            Name = venue.Name,
            Description = venue.Description ?? string.Empty, // Null gelirse boş string gönder
            ImageUrl = venue.ProfilePictureUrl ?? string.Empty // Direkt entity'den al
        };

        await _eventPublisher.PublishAsync(integrationEvent, cancellationToken);
        // -------------------------

        return Unit.Value;
    }
}