using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.ActivateVenue;

public sealed class ActivateVenueCommandHandler
    : IRequestHandler<ActivateVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly ICityDiscoveryService _cityDiscoveryService; 

    public ActivateVenueCommandHandler(IVenueRepository venueRepository, ICityDiscoveryService cityDiscoveryService)
    {
        _venueRepository = venueRepository;
        _cityDiscoveryService = cityDiscoveryService;
    }

    public async Task<Unit> Handle(ActivateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue not found with id: {request.VenueId}");

        // Domain davranışı
        venue.Activate();

        await _venueRepository.UpdateAsync(venue, cancellationToken);
        var venueForElastic = await _venueRepository.GetVenueWithDetailsAsync(request.VenueId, cancellationToken);

        if (venueForElastic != null && venueForElastic.IsApproved)
        {
            await _cityDiscoveryService.IndexVenueWithDetailsAsync(venueForElastic);
        }
        return Unit.Value;
    }
}
