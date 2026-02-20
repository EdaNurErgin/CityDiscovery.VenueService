using CityDiscovery.AdminNotificationService.Shared.Common.Events.AdminNotification;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using MassTransit;


namespace CityDiscovery.VenueService.Venues.Infrastructure.MessageBus.Consumers
{
    //  IConsumer<ContentRemovedEvent>" eklendi
    public class ContentRemovedConsumer : IConsumer<ContentRemovedEvent>
    {
        // Context değişkeni tanımlandı
        private readonly VenueDbContext _context;

        // Constructor (Yapıcı Metot) eklendi
        public ContentRemovedConsumer(VenueDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<ContentRemovedEvent> context)
        {
            var message = context.Message;

            // Mekan silme  
            if (message.ContentType == "Venue")
            {
               
                var venue = await _context.Venues.FindAsync(message.ContentId);

                if (venue != null)
                {
                    _context.Venues.Remove(venue);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"[VenueService] Mekan silindi: {message.ContentId}");
                }
            }
        }
    }
}