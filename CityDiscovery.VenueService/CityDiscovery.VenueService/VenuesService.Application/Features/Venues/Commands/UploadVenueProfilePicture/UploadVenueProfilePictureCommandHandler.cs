using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Application.Interfaces.Services;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UploadVenueProfilePicture;

public sealed class UploadVenueProfilePictureCommandHandler
    : IRequestHandler<UploadVenueProfilePictureCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IFileStorageService _fileStorageService;

    public UploadVenueProfilePictureCommandHandler(
        IVenueRepository venueRepository,
        IFileStorageService fileStorageService)
    {
        _venueRepository = venueRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Unit> Handle(
        UploadVenueProfilePictureCommand request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue is null)
            throw new KeyNotFoundException($"Venue with id '{request.VenueId}' was not found.");

        // Dosya yolu örneği: "venues/{venueId}/profile.jpg"
        // Aynı isimle kaydedersek eskisi ezilir (overwrite), bu da tam istediğimiz şey (tek profil fotosu).
        // Veya unique isim verip eskisini silebilirsin. Basitlik için overwrite mantığı:
        var extension = Path.GetExtension(request.FileName);
        var relativePath = $"venues/{venue.Id}/profile{extension}";

        var url = await _fileStorageService.SaveFileAsync(
            request.FileStream,
            relativePath, // İsimlendirmeyi IFileStorageService içinde unique yapıyorsa orayı düzenlemen gerekebilir*
            request.ContentType,
            cancellationToken);

        // Domain metodunu çağır
        venue.UpdateProfilePicture(url);

        await _venueRepository.UpdateAsync(venue, cancellationToken);

        return Unit.Value;
    }
}