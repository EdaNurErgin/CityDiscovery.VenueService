using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.ValueObjects;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateVenueBasicInfo;

public sealed class UpdateVenueBasicInfoCommandHandler
    : IRequestHandler<UpdateVenueBasicInfoCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;

    public UpdateVenueBasicInfoCommandHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<Unit> Handle(UpdateVenueBasicInfoCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue not found with id: {request.VenueId}");

        var priceLevel = request.PriceLevel.HasValue
            ? PriceLevel.Create(request.PriceLevel.Value)
            : null;

        // GeoLocation yok artık → direkt latitude/longitude veriyoruz
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

        return Unit.Value;
    }
}
