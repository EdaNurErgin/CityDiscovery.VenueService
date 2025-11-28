using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenueEvents;

public sealed record GetVenueEventsQuery(Guid VenueId)
    : IRequest<IReadOnlyList<EventDto>>;
