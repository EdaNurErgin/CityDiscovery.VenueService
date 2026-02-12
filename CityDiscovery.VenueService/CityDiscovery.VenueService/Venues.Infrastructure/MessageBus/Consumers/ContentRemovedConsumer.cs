using CityDiscovery.AdminNotificationService.Shared.Common.Events.AdminNotification;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using MassTransit;


namespace CityDiscovery.VenueService.Venues.Infrastructure.MessageBus.Consumers
{
    // 1. DÜZELTME: ": IConsumer<ContentRemovedEvent>" eklendi
    public class ContentRemovedConsumer : IConsumer<ContentRemovedEvent>
    {
        // 2. DÜZELTME: Context değişkeni tanımlandı
        private readonly VenueDbContext _context;

        // 3. DÜZELTME: Constructor (Yapıcı Metot) eklendi
        public ContentRemovedConsumer(VenueDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<ContentRemovedEvent> context)
        {
            var message = context.Message;

            // Sadece Mekan silme işi burada kaldı:
            if (message.ContentType == "Venue")
            {
                // Artık _context hata vermez
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