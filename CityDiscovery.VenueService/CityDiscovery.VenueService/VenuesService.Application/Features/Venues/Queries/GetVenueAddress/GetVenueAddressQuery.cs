using MediatR;
using CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue;

namespace CityDiscovery.VenueService.VenuesService.Application.Features.Venues.Queries.GetVenueAddress
{
    public class GetVenueAddressQuery : IRequest<VenueAddressDto>
    {
        public Guid VenueId { get; set; }

        public GetVenueAddressQuery(Guid venueId)
        {
            VenueId = venueId;
        }
    }
}