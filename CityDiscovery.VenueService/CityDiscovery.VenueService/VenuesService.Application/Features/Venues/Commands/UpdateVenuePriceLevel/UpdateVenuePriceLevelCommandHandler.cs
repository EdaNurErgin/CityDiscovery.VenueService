using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateVenuePriceLevel;

public class UpdateVenuePriceLevelCommandHandler : IRequestHandler<UpdateVenuePriceLevelCommand>
{
    private readonly IVenueRepository _venueRepository;

    public UpdateVenuePriceLevelCommandHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task Handle(UpdateVenuePriceLevelCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue == null)
            throw new KeyNotFoundException("Mekan bulunamadı.");

        // Domain metodu çağrılıyor
        venue.UpdatePriceLevel(request.PriceLevel);

        await _venueRepository.UpdateAsync(venue, cancellationToken);
    }
}