using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetMenuForVenue;

public sealed class GetMenuForVenueQueryHandler
    : IRequestHandler<GetMenuForVenueQuery, IReadOnlyList<MenuCategoryDto>>
{
    private readonly IVenueRepository _venueRepository;

    public GetMenuForVenueQueryHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<IReadOnlyList<MenuCategoryDto>> Handle(
        GetMenuForVenueQuery request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue with id '{request.VenueId}' was not found.");

        var categories = venue.MenuCategories
            .OrderBy(mc => mc.SortOrder)
            .Select(mc => new MenuCategoryDto
            {
                MenuCategoryId = mc.MenuCategoryId,
                Name = mc.Name,
                SortOrder = mc.SortOrder,
                IsActive = mc.IsActive,
                Items = mc.Items
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new MenuItemDto
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Price = i.Price,
                        ImageUrl = i.ImageUrl,
                        IsAvailable = i.IsAvailable,
                        SortOrder = i.SortOrder
                    })
                    .ToList()
            })
            .ToList()
            .AsReadOnly();

        return categories;
    }
}
