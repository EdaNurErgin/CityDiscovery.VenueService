using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenuePhotos;

public sealed record GetVenuePhotosQuery(Guid VenueId)
    : IRequest<IReadOnlyList<VenuePhotoDto>>;
