using CityDiscovery.Venues.Application.Features.Categories.Queries.GetAllCategories;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenueCategories;

public sealed class GetVenueCategoriesQueryHandler
    : IRequestHandler<GetVenueCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IVenueRepository _venueRepository;

    public GetVenueCategoriesQueryHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(
        GetVenueCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
        {
            // İstersen kendi NotFoundException tipin varsa onu kullan
            throw new KeyNotFoundException($"Venue with id '{request.VenueId}' was not found.");
        }

        // Venue.VenueCategories -> VenueCategory -> Category
        var categories = venue.VenueCategories
            .Where(vc => vc.Category is not null)
            .Select(vc => vc.Category!)
            .ToList();

        var result = categories
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Slug = c.Slug,
                IconUrl = c.IconUrl
            })
            .ToList()
            .AsReadOnly();

        return result;
    }
}
