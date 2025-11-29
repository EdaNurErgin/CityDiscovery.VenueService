using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteVenue;

public sealed record DeleteVenueCommand(Guid VenueId) : IRequest<Unit>;

