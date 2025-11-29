namespace CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Category;

public sealed class CategoryDto
{
    public int CategoryId { get; init; }
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string? IconUrl { get; init; }
}