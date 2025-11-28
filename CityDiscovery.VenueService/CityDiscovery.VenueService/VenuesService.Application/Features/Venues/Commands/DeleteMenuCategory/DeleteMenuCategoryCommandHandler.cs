using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteMenuCategory;

public sealed class DeleteMenuCategoryCommandHandler
    : IRequestHandler<DeleteMenuCategoryCommand, Unit>
{
    private readonly IMenuCategoryRepository _menuCategoryRepository;

    public DeleteMenuCategoryCommandHandler(IMenuCategoryRepository menuCategoryRepository)
    {
        _menuCategoryRepository = menuCategoryRepository;
    }

    public async Task<Unit> Handle(
        DeleteMenuCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _menuCategoryRepository.GetByIdAsync(request.MenuCategoryId, cancellationToken);
        if (category is null)
            return Unit.Value; // sessiz çık

        await _menuCategoryRepository.DeleteAsync(category, cancellationToken);

        return Unit.Value;
    }
}
