using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.CreateEvent;

public sealed class CreateEventCommandHandler
    : IRequestHandler<CreateEventCommand, Guid>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IEventRepository _eventRepository;

    public CreateEventCommandHandler(
        IVenueRepository venueRepository,
        IEventRepository eventRepository)
    {
        _venueRepository = venueRepository;
        _eventRepository = eventRepository;
    }

    public async Task<Guid> Handle(
        CreateEventCommand request,
        CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue is null)
            throw new KeyNotFoundException($"Venue with id '{request.VenueId}' was not found.");

        var ev = Event.Create(
            request.VenueId,
            request.Title,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.ImageUrl);

        await _eventRepository.AddAsync(ev, cancellationToken);

        return ev.Id;
    }
}
