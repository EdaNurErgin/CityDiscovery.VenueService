using MediatR;
using System.IO;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UploadVenueProfilePicture;

public sealed record UploadVenueProfilePictureCommand(
    Guid VenueId,
    Stream FileStream,
    string FileName,
    string ContentType
) : IRequest<Unit>;