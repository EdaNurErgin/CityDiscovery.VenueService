using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.AddCategoryToVenue;

public sealed record AddCategoryToVenueCommand(
    Guid VenueId,
    int CategoryId
) : IRequest<Unit>;
