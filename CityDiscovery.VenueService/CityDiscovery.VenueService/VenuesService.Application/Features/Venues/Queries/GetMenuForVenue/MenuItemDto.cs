namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetMenuForVenue;

public sealed class MenuItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public decimal? Price { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsAvailable { get; init; }
    public int SortOrder { get; init; }
}
