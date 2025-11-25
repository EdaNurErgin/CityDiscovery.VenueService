using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.RemoveCategoryFromVenue;

public sealed record RemoveCategoryFromVenueCommand(
    Guid VenueId,
    int CategoryId
) : IRequest<Unit>;
