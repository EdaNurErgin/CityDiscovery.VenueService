using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.CreateMenuCategory;

public sealed record CreateMenuCategoryCommand(
    Guid VenueId,
    string Name,
    int? SortOrder
) : IRequest<int>; // MenuCategoryId döneceğiz
