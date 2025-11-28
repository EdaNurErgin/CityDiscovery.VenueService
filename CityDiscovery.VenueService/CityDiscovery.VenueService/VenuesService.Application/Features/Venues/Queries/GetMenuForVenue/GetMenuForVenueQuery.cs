using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetMenuForVenue;

public sealed record GetMenuForVenueQuery(Guid VenueId)
    : IRequest<IReadOnlyList<MenuCategoryDto>>;
