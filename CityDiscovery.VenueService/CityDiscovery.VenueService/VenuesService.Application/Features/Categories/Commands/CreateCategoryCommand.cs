using MediatR;

namespace CityDiscovery.Venues.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? IconUrl,
    Guid UserId,
    string UserRole) : IRequest<int>;