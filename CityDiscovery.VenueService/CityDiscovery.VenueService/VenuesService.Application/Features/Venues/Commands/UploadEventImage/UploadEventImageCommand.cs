using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UploadEventImage;

public sealed record UploadEventImageCommand(
    Guid EventId,
    Stream FileStream,
    string FileName,
    string ContentType
) : IRequest<Unit>;
