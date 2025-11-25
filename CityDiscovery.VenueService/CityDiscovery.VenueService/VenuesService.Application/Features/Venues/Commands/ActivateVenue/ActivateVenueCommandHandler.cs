using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.ActivateVenue;

public sealed class ActivateVenueCommandHandler
    : IRequestHandler<ActivateVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;

    public ActivateVenueCommandHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<Unit> Handle(ActivateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue not found with id: {request.VenueId}");

        // Domain davranışı
        venue.Activate();

        await _venueRepository.UpdateAsync(venue, cancellationToken);

        return Unit.Value;
    }
}
