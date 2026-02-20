namespace CityDiscovery.Venues.API.Models.Requests;

public sealed record CreateCategoryRequest(
    string Name,
    string? IconUrl = null);