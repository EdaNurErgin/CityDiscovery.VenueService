using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateVenuePriceLevel;

public class UpdateVenuePriceLevelCommandHandler : IRequestHandler<UpdateVenuePriceLevelCommand>
{
    private readonly IVenueRepository _venueRepository;
    private readonly ICityDiscoveryService _cityDiscoveryService; 

    public UpdateVenuePriceLevelCommandHandler(IVenueRepository venueRepository, ICityDiscoveryService cityDiscoveryService)
    {
        _venueRepository = venueRepository;
        _cityDiscoveryService = cityDiscoveryService;
    }

    public async Task Handle(UpdateVenuePriceLevelCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue == null)
            throw new KeyNotFoundException("Mekan bulunamadı.");

        // Domain metodu çağrılıyor
        venue.UpdatePriceLevel(request.PriceLevel);

        await _venueRepository.UpdateAsync(venue, cancellationToken);
        var venueForElastic = await _venueRepository.GetVenueWithDetailsAsync(request.VenueId, cancellationToken);
        if (venueForElastic != null && venueForElastic.IsApproved)
        {
            await _cityDiscoveryService.IndexVenueWithDetailsAsync(venueForElastic);
        }
    }
}