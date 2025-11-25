using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Domain.ValueObjects;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.CreateVenue
{
    public sealed class CreateVenueCommandHandler
        : IRequestHandler<CreateVenueCommand, Guid>
    {
        private readonly IVenueRepository _venueRepository;

        public CreateVenueCommandHandler(
            IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }


        public async Task<Guid> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
        {
            // 1. Owner zaten bir mekana sahip mi?
            if (await _venueRepository.OwnerHasVenueAsync(request.OwnerUserId, cancellationToken))
                throw new InvalidOperationException("Owner already has a venue.");

            // 2. PriceLevel value object
            var price = PriceLevel.FromNullable(request.PriceLevel);

            // 3. Domain entity creation (GeoLocation yok, direkt lat/lon)
            var venue = Venuex.Create(
                request.OwnerUserId,
                request.Name,
                request.Description,
                request.AddressText,
                request.Phone,
                request.WebsiteUrl,
                price,
                request.OpeningHoursJson,
                request.Latitude,   // ✅
                request.Longitude   // ✅
            );

            // 4. Persist
            await _venueRepository.AddAsync(venue, cancellationToken);

            return venue.Id;
        }
    }
}
