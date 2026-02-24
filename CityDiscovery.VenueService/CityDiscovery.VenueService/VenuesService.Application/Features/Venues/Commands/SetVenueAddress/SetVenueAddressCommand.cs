using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.SetVenueAddress;

public sealed record SetVenueAddressCommand(
    Guid VenueId,
    int CountryId,
    int CityId,
    int DistrictId,
    string? Neighborhood, 
    string? Street,       
    string? BuildingNo,
    string? FullAddress
) : IRequest;