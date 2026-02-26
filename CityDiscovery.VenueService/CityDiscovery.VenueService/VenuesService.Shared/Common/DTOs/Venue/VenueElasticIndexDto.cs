namespace CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue
{
    public class VenueElasticIndexDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; } 
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? VenueCategory { get; set; }
        public byte? PriceLevel { get; set; }
        public bool IsActive { get; set; }
        public double AverageRating { get; set; }
        public LocationDto Location { get; set; }
    }
}
