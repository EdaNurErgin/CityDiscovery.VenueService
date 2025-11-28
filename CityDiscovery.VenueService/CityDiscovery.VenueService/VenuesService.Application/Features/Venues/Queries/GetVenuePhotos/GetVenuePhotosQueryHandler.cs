using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenuePhotos;

public sealed class GetVenuePhotosQueryHandler
    : IRequestHandler<GetVenuePhotosQuery, IReadOnlyList<VenuePhotoDto>>
{
    private readonly IVenueRepository _venueRepository;

    public GetVenuePhotosQueryHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<IReadOnlyList<VenuePhotoDto>> Handle(
        GetVenuePhotosQuery request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue with id '{request.VenueId}' was not found.");

        var photos = venue.Photos
            .OrderBy(p => p.SortOrder)
            .Select(p => new VenuePhotoDto
            {
                Id = p.Id,
                Url = p.Url,
                Caption = p.Caption,
                SortOrder = p.SortOrder
            })
            .ToList()
            .AsReadOnly();

        return photos;
    }
}
