using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateVenuePriceLevel;

public sealed record UpdateVenuePriceLevelCommand(
    Guid VenueId,
    byte PriceLevel
) : IRequest;