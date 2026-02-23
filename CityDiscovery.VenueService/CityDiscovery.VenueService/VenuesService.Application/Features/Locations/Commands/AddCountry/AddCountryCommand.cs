using MediatR;

namespace CityDiscovery.Venues.Application.Features.Locations.Commands.AddCountry;

public sealed record AddCountryCommand(
    string Name,
    string? Code
) : IRequest<int>;