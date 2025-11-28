using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateMenuCategory;

public sealed record UpdateMenuCategoryCommand(
    int MenuCategoryId,
    string Name,
    int SortOrder,
    bool IsActive
) : IRequest<Unit>;
