using MediatR;
using System.IO;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UploadVenuePhoto;

public sealed record UploadVenuePhotoCommand(
    Guid VenueId,
    Stream FileStream,
    string FileName,
    string ContentType,
    string? Caption,
    int? SortOrder
) : IRequest<Guid>;
