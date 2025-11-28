using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteMenuCategory;

public sealed record DeleteMenuCategoryCommand(int MenuCategoryId) : IRequest<Unit>;
