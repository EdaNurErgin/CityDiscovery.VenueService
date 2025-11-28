using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Application.Interfaces.Services;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UploadMenuItemPhoto;

public sealed class UploadMenuItemPhotoCommandHandler
    : IRequestHandler<UploadMenuItemPhotoCommand, Unit>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IFileStorageService _fileStorageService;

    public UploadMenuItemPhotoCommandHandler(
        IMenuItemRepository menuItemRepository,
        IFileStorageService fileStorageService)
    {
        _menuItemRepository = menuItemRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Unit> Handle(UploadMenuItemPhotoCommand request, CancellationToken cancellationToken)
    {
        var item = await _menuItemRepository.GetByIdAsync(request.MenuItemId, cancellationToken);
        if (item is null)
            throw new KeyNotFoundException($"MenuItem with id '{request.MenuItemId}' was not found.");

        // Örn: "menu-items/{id}/filename.png" gibi bir path
        var relativePath = $"menu-items/{item.Id}/{request.FileName}";

        var imageUrl = await _fileStorageService.SaveFileAsync(
            request.FileStream,
            relativePath,
            request.ContentType,
            cancellationToken);

        // Domain davranışıyla imageUrl güncelle
        item.Update(
            item.Name,
            item.Description,
            item.Price,
            imageUrl,
            item.IsAvailable,
            item.SortOrder);

        await _menuItemRepository.UpdateAsync(item, cancellationToken);

        return Unit.Value;
    }
}
