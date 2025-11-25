using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.ActivateVenue;

public sealed record ActivateVenueCommand(Guid VenueId) : IRequest<Unit>;
