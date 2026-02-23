using MediatR;

namespace CityDiscovery.Venues.Application.Features.Locations.Commands.Delete;

public sealed record DeleteCountryCommand(int CountryId) : IRequest<Unit>;
public sealed record DeleteCityCommand(int CityId) : IRequest<Unit>;
public sealed record DeleteDistrictCommand(int DistrictId) : IRequest<Unit>;