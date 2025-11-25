using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.DeactivateVenue;

public sealed record DeactivateVenueCommand(Guid VenueId) : IRequest<Unit>;
