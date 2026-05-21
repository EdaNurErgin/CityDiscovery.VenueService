using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;
using MediatR;

namespace CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.GetVenueAddress
{
    public class GetVenueAddressQueryHandler : IRequestHandler<GetVenueAddressQuery, VenueAddressDto>
    {
        private readonly IVenueRepository _venueRepository;

        public GetVenueAddressQueryHandler(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task<VenueAddressDto> Handle(GetVenueAddressQuery request, CancellationToken cancellationToken)
        {
            // Mekanı adresi ve detaylarıyla birlikte getiriyoruz (Önceki adımda Include eklediğimiz metot)
            var venue = await _venueRepository.GetVenueWithDetailsAsync(request.VenueId, cancellationToken);

            if (venue == null || venue.Address == null)
            {
                return null;
            }

            return new VenueAddressDto
            {
                VenueId = venue.Id,
                CountryId = venue.Address.CountryId,
                CityId = venue.Address.CityId,
                DistrictId = venue.Address.DistrictId,
                Neighborhood = venue.Address.Neighborhood,
                Street = venue.Address.Street,
                BuildingNo = venue.Address.BuildingNo,
                FullAddress = venue.Address.FullAddress,

                // Koordinatlar adres tablosunda değil, ana Venue tablosunda (Geography tipi olarak) duruyor
                Latitude = venue.Location?.Y,
                Longitude = venue.Location?.X
            };
        }
    }
}