using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.AddPhotoToVenue;

public sealed record AddPhotoToVenueCommand(
    Guid VenueId,
    string Url,
    string? Caption,
    int? SortOrder
) : IRequest<Guid>; // Foto Id döndürelim
