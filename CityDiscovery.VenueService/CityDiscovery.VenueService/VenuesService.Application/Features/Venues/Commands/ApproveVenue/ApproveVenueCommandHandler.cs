using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.ApproveVenue;

public sealed class ApproveVenueCommandHandler: IRequestHandler<ApproveVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;

    public ApproveVenueCommandHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<Unit> Handle(ApproveVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue not found with id: {request.VenueId}");

        // Domain davranışı: Onayla
        venue.Approve();

        await _venueRepository.UpdateAsync(venue, cancellationToken);

        return Unit.Value;
    }
}
