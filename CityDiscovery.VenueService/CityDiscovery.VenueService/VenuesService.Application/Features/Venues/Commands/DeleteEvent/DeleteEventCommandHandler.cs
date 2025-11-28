using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.DeleteEvent;

public sealed class DeleteEventCommandHandler
    : IRequestHandler<DeleteEventCommand, Unit>
{
    private readonly IEventRepository _eventRepository;

    public DeleteEventCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<Unit> Handle(
        DeleteEventCommand request,
        CancellationToken cancellationToken)
    {
        var ev = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (ev is null)
            return Unit.Value;

        await _eventRepository.DeleteAsync(ev, cancellationToken);
        return Unit.Value;
    }
}
