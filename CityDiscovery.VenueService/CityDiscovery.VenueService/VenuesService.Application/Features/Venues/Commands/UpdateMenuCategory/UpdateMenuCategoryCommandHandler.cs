using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateMenuCategory;

public sealed class UpdateMenuCategoryCommandHandler
    : IRequestHandler<UpdateMenuCategoryCommand, Unit>
{
    private readonly IMenuCategoryRepository _menuCategoryRepository;

    public UpdateMenuCategoryCommandHandler(IMenuCategoryRepository menuCategoryRepository)
    {
        _menuCategoryRepository = menuCategoryRepository;
    }

    public async Task<Unit> Handle(
        UpdateMenuCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _menuCategoryRepository.GetByIdAsync(request.MenuCategoryId, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"MenuCategory with id '{request.MenuCategoryId}' was not found.");

        category.Rename(request.Name);
        category.ChangeSortOrder(request.SortOrder);

        if (request.IsActive)
            category.Activate();
        else
            category.Deactivate();

        await _menuCategoryRepository.UpdateAsync(category, cancellationToken);

        return Unit.Value;
    }
}
