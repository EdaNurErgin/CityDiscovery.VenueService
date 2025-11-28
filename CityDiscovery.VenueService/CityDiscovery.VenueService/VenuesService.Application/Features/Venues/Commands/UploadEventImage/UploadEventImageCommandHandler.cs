using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Application.Interfaces.Services;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UploadEventImage;

public sealed class UploadEventImageCommandHandler
    : IRequestHandler<UploadEventImageCommand, Unit>
{
    private readonly IEventRepository _eventRepository;
    private readonly IFileStorageService _fileStorageService;

    public UploadEventImageCommandHandler(
        IEventRepository eventRepository,
        IFileStorageService fileStorageService)
    {
        _eventRepository = eventRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Unit> Handle(
        UploadEventImageCommand request,
        CancellationToken cancellationToken)
    {
        var ev = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (ev is null)
            throw new KeyNotFoundException($"Event with id '{request.EventId}' was not found.");

        // Kaydedilecek dosya yolu (örnek)
        var relativePath = $"events/{ev.VenueId}/{ev.Id}/{request.FileName}";

        var imageUrl = await _fileStorageService.SaveFileAsync(
            request.FileStream,
            relativePath,
            request.ContentType,
            cancellationToken);

        // Domain davranışı ile imageUrl güncelle
        ev.UpdateImage(imageUrl);

        await _eventRepository.UpdateAsync(ev, cancellationToken);

        return Unit.Value;
    }
}
