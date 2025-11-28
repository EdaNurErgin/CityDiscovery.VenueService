namespace CityDiscovery.VenueService.VenuesService.API.Models.Requests
{
    public sealed class UpdateEventRequest
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
