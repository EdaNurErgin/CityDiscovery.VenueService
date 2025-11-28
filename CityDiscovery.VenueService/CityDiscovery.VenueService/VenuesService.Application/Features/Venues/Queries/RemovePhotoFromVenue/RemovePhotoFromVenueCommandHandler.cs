using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.RemovePhotoFromVenue;

public sealed class RemovePhotoFromVenueCommandHandler
    : IRequestHandler<RemovePhotoFromVenueCommand, Unit>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenuePhotoRepository _venuePhotoRepository;

    public RemovePhotoFromVenueCommandHandler(
        IVenueRepository venueRepository,
        IVenuePhotoRepository venuePhotoRepository)
    {
        _venueRepository = venueRepository;
        _venuePhotoRepository = venuePhotoRepository;
    }

    public async Task<Unit> Handle(
        RemovePhotoFromVenueCommand request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue is null)
            throw new KeyNotFoundException($"Venue with id '{request.VenueId}' was not found.");

        var photo = await _venuePhotoRepository.GetByIdAsync(request.PhotoId, cancellationToken);
        if (photo is null || photo.VenueId != request.VenueId)
            return Unit.Value; // sessizce çık

        await _venuePhotoRepository.RemoveAsync(photo, cancellationToken);

        return Unit.Value;
    }
}
