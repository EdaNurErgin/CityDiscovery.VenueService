using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteEvent;

public sealed record DeleteEventCommand(Guid EventId) : IRequest<Unit>;
