using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.ApproveVenue;

public sealed record ApproveVenueCommand(Guid VenueId) : IRequest<Unit>;

