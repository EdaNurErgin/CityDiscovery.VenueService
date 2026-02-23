using MediatR;

namespace CityDiscovery.Venues.Application.Features.Locations.Commands.AddDistrict;

public sealed record AddDistrictCommand(
    int CityId,
    string Name
) : IRequest<int>;