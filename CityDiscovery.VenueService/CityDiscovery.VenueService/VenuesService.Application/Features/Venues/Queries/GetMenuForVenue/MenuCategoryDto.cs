namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetMenuForVenue;

public sealed class MenuCategoryDto
{
    public int MenuCategoryId { get; init; }
    public string Name { get; init; } = default!;
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }

    public IReadOnlyList<MenuItemDto> Items { get; init; } = Array.Empty<MenuItemDto>();
}
