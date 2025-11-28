using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteMenuItem;

public sealed class DeleteMenuItemCommandHandler
    : IRequestHandler<DeleteMenuItemCommand, Unit>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public DeleteMenuItemCommandHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<Unit> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _menuItemRepository.GetByIdAsync(request.MenuItemId, cancellationToken);
        if (item is null)
            return Unit.Value; // sessizce ç?k

        await _menuItemRepository.DeleteAsync(item, cancellationToken);

        return Unit.Value;
    }
}
