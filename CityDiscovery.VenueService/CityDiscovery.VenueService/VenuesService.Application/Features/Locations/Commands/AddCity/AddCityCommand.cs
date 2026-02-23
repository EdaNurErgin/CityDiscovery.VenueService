using MediatR;

namespace CityDiscovery.Venues.Application.Features.Locations.Commands.AddCity;

public sealed record AddCityCommand(
    int CountryId,
    string Name
) : IRequest<int>;