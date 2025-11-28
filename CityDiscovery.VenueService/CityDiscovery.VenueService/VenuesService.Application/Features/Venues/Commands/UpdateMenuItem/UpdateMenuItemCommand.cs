using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateMenuItem;

public sealed record UpdateMenuItemCommand(
    Guid MenuItemId,
    string Name,
    string? Description,
    decimal? Price,
    string? ImageUrl,
    bool IsAvailable,
    int SortOrder
) : IRequest<Unit>;
