using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Application.Interfaces.Services;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UploadMenuItemPhoto;

public sealed record UploadMenuItemPhotoCommand(
    Guid MenuItemId,
    Stream FileStream,
    string FileName,
    string ContentType
) : IRequest<Unit>;
