using MediatR;
using System;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteMenuItem;

public sealed record DeleteMenuItemCommand(Guid MenuItemId) : IRequest<Unit>;
