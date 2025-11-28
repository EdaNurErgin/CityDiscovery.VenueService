namespace CityDiscovery.Venues.Application.Features.Venues.Queries.GetVenueEvents;

public sealed class EventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ImageUrl { get; set; }
}
