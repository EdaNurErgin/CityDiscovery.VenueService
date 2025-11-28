using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.CreateMenuItem;

public sealed class CreateMenuItemCommandHandler
    : IRequestHandler<CreateMenuItemCommand, Guid>
{
    private readonly IMenuCategoryRepository _menuCategoryRepository;
    private readonly IMenuItemRepository _menuItemRepository;

    public CreateMenuItemCommandHandler(
        IMenuCategoryRepository menuCategoryRepository,
        IMenuItemRepository menuItemRepository)
    {
        _menuCategoryRepository = menuCategoryRepository;
        _menuItemRepository = menuItemRepository;
    }

    public async Task<Guid> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var category = await _menuCategoryRepository.GetByIdAsync(request.MenuCategoryId, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"MenuCategory with id '{request.MenuCategoryId}' was not found.");

        int sortOrder = request.SortOrder ??
                        (category.Items.Any()
                            ? category.Items.Max(i => i.SortOrder) + 1
                            : 0);

        var item = MenuItem.Create(
            request.MenuCategoryId,
            request.Name,
            request.Description,
            request.Price,
            request.ImageUrl,
            sortOrder);

        await _menuItemRepository.AddAsync(item, cancellationToken);

        return item.Id;
    }
}
