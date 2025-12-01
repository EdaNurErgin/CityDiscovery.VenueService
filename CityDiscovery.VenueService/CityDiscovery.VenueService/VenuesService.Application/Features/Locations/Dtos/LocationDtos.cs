namespace CityDiscovery.Venues.Application.Features.Locations.Dtos;

public record CountryDto(int Id, string Name, string? Code);
public record CityDto(int Id, string Name);
public record DistrictDto(int Id, string Name);