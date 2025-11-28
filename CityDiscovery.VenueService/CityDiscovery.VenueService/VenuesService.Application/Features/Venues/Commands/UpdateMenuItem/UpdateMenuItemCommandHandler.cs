using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateMenuItem;

public sealed class UpdateMenuItemCommandHandler
    : IRequestHandler<UpdateMenuItemCommand, Unit>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public UpdateMenuItemCommandHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<Unit> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _menuItemRepository.GetByIdAsync(request.MenuItemId, cancellationToken);
        if (item is null)
            throw new KeyNotFoundException($"MenuItem with id '{request.MenuItemId}' was not found.");

        item.Update(
            request.Name,
            request.Description,
            request.Price,
            request.ImageUrl,
            request.IsAvailable,
            request.SortOrder);

        await _menuItemRepository.UpdateAsync(item, cancellationToken);

        return Unit.Value;
    }
}
