using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.UpdateEvent;

public sealed class UpdateEventCommandHandler
    : IRequestHandler<UpdateEventCommand, Unit>
{
    private readonly IEventRepository _eventRepository;

    public UpdateEventCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<Unit> Handle(
        UpdateEventCommand request,
        CancellationToken cancellationToken)
    {
        var ev = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (ev is null)
            throw new KeyNotFoundException($"Event with id '{request.EventId}' was not found.");

        ev.Update(
            request.Title,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.ImageUrl,
            request.IsActive);

        await _eventRepository.UpdateAsync(ev, cancellationToken);

        return Unit.Value;
    }
}
