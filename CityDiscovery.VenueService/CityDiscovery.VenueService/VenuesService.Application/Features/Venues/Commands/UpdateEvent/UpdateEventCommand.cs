using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateEvent;

public sealed record UpdateEventCommand(
    Guid EventId,
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime? EndDate,
    string? ImageUrl,
    bool IsActive
) : IRequest<Unit>;
