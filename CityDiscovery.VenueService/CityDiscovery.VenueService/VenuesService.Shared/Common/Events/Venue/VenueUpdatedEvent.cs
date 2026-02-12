using VenuesService.Shared.Common.Events; // IIntegrationEvent'in olduğu namespace

namespace CityDiscovery.VenuesService.Shared.Common.Events.Venue
{
    public class VenueUpdatedEvent : IIntegrationEvent
    {
        // IIntegrationEvent gereksinimleri (Event Bus için gerekli)
        public Guid Id { get; private set; } = Guid.NewGuid();
        public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;

        // Güncellenen veriler (Handler'dan gelenler)
        public Guid VenueId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}