using MediatR;

public sealed record CreateMenuItemCommand(
    int MenuCategoryId,
    string Name,
    string? Description,
    decimal? Price,
    string? ImageUrl,
    int? SortOrder
) : IRequest<Guid>;
