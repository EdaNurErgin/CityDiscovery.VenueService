using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.CreateEvent;

public sealed record CreateEventCommand(
    Guid VenueId,
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime? EndDate,
    string? ImageUrl
) : IRequest<Guid>;
