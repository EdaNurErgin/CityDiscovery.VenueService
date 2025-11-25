
using MediatR;
using System.Collections.Generic;

namespace CityDiscovery.Venues.Application.Features.Categories.Queries.GetAllCategories;

public sealed record GetAllCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;
