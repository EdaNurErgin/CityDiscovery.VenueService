using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.AddPhotoToVenue;

public sealed class AddPhotoToVenueCommandHandler
    : IRequestHandler<AddPhotoToVenueCommand, Guid>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenuePhotoRepository _venuePhotoRepository;

    public AddPhotoToVenueCommandHandler(
        IVenueRepository venueRepository,
        IVenuePhotoRepository venuePhotoRepository)
    {
        _venueRepository = venueRepository;
        _venuePhotoRepository = venuePhotoRepository;
    }

    public async Task<Guid> Handle(
        AddPhotoToVenueCommand request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue is null)
            throw new KeyNotFoundException($"Venue with id '{request.VenueId}' was not found.");

        // SortOrder null ise, en sona ekleyelim
        int sortOrder = request.SortOrder ??
                        (venue.Photos.Any() ? venue.Photos.Max(p => p.SortOrder) + 1 : 0);

        var photo = VenuePhoto.Create(
            request.VenueId,
            request.Url,
            request.Caption,
            sortOrder);

        await _venuePhotoRepository.AddAsync(photo, cancellationToken);

        return photo.Id;
    }
}
