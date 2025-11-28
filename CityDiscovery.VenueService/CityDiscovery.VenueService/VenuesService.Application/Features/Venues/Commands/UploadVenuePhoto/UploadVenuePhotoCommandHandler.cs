using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Application.Interfaces.Services;
using CityDiscovery.Venues.Domain.Entities;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UploadVenuePhoto;

public sealed class UploadVenuePhotoCommandHandler
    : IRequestHandler<UploadVenuePhotoCommand, Guid>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenuePhotoRepository _venuePhotoRepository;
    private readonly IFileStorageService _fileStorageService;

    public UploadVenuePhotoCommandHandler(
        IVenueRepository venueRepository,
        IVenuePhotoRepository venuePhotoRepository,
        IFileStorageService fileStorageService)
    {
        _venueRepository = venueRepository;
        _venuePhotoRepository = venuePhotoRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Guid> Handle(
        UploadVenuePhotoCommand request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue is null)
        {
            throw new KeyNotFoundException($"Venue with id '{request.VenueId}' was not found.");
        }

        // 1) Dosyayı storage'a kaydet → URL al
        var url = await _fileStorageService.SaveFileAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            cancellationToken);

        // 2) SortOrder null ise en sona ekle
        int sortOrder = request.SortOrder ??
                        (venue.Photos.Any() ? venue.Photos.Max(p => p.SortOrder) + 1 : 0);

        // 3) Domain entity oluştur
        var photo = VenuePhoto.Create(
            request.VenueId,
            url,
            request.Caption,
            sortOrder);

        // 4) DB'ye kaydet
        await _venuePhotoRepository.AddAsync(photo, cancellationToken);

        return photo.Id;
    }
}
