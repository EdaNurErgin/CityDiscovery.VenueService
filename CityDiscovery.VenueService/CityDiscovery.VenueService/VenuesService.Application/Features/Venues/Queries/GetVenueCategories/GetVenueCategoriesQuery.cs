using CityDiscovery.Venues.Application.Features.Categories.Queries.GetAllCategories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenueCategories;

public sealed record GetVenueCategoriesQuery(Guid VenueId)
    : IRequest<IReadOnlyList<CategoryDto>>;
