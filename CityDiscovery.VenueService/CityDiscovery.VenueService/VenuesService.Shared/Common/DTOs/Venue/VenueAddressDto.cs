namespace CityDiscovery.VenueService.VenuesService.Shared.Common.DTOs.Venue
{
    public class VenueAddressDto
    {
        public Guid VenueId { get; set; }

        // ID'ler Guid değil, int olmalı
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }

        // Senin entity'ndeki gerçek property'ler
        public string Neighborhood { get; set; }
        public string Street { get; set; }
        public string BuildingNo { get; set; }
        public string FullAddress { get; set; }

        // Harita için
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}