using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.CreateMenuCategory;

public sealed class CreateMenuCategoryCommandHandler
    : IRequestHandler<CreateMenuCategoryCommand, int>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMenuCategoryRepository _menuCategoryRepository;

    public CreateMenuCategoryCommandHandler(
        IVenueRepository venueRepository,
        IMenuCategoryRepository menuCategoryRepository)
    {
        _venueRepository = venueRepository;
        _menuCategoryRepository = menuCategoryRepository;
    }

    public async Task<int> Handle(
        CreateMenuCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue is null)
            throw new KeyNotFoundException($"Venue with id '{request.VenueId}' was not found.");

        int sortOrder = request.SortOrder ??
                        (venue.MenuCategories.Any()
                            ? venue.MenuCategories.Max(mc => mc.SortOrder) + 1
                            : 0);

        var category = MenuCategory.Create(request.VenueId, request.Name, sortOrder);

        await _menuCategoryRepository.AddAsync(category, cancellationToken);

        return category.MenuCategoryId;
    }
}
