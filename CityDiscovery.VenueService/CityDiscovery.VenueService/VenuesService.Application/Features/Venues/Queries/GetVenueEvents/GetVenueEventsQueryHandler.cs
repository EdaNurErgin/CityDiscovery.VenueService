using CityDiscovery.Venues.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenueEvents;

public sealed class GetVenueEventsQueryHandler
    : IRequestHandler<GetVenueEventsQuery, IReadOnlyList<EventDto>>
{
    private readonly IEventRepository _eventRepository;

    public GetVenueEventsQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    //public async Task<IReadOnlyList<EventDto>> Handle(
    //    GetVenueEventsQuery request,
    //    CancellationToken cancellationToken)
    //{
    //    var events = await _eventRepository
    //        .GetActiveEventsForVenueAsync(request.VenueId, null, cancellationToken);

    //    return events
    //        .Select(e => new EventDto
    //        {
    //            Id = e.Id,
    //            Title = e.Title,
    //            Description = e.Description,
    //            StartDate = e.StartDate,
    //            EndDate = e.EndDate,
    //            ImageUrl = e.ImageUrl
    //        })
    //        .ToList()
    //        .AsReadOnly();
    //}

    public async Task<IReadOnlyList<EventDto>> Handle(
    GetVenueEventsQuery request,
    CancellationToken cancellationToken)
    {
        var events = await _eventRepository
            .GetEventsForVenueAsync(request.VenueId, cancellationToken);

        return events
            .Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                ImageUrl = e.ImageUrl
            })
            .ToList()
            .AsReadOnly();
    }

}
