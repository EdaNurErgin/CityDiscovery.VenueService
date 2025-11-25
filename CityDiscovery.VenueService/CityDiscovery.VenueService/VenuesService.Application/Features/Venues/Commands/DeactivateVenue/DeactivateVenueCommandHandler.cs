using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.DeactivateVenue;

public sealed class DeactivateVenueCommandHandler
    : IRequestHandler<DeactivateVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;

    public DeactivateVenueCommandHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<Unit> Handle(DeactivateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue not found with id: {request.VenueId}");

        // Domain davranışı
        venue.Deactivate();

        await _venueRepository.UpdateAsync(venue, cancellationToken);

        return Unit.Value;
    }
}
