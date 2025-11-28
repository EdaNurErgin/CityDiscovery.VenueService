using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.RemovePhotoFromVenue;

public sealed record RemovePhotoFromVenueCommand(
    Guid VenueId,
    Guid PhotoId
) : IRequest<Unit>;
