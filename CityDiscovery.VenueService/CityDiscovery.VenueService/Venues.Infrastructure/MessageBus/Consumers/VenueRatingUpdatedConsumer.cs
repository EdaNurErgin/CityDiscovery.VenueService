//using CityDiscovery.Venues.Application.Interfaces.Repositories;
//using CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Review;
//using MassTransit;

//namespace CityDiscovery.VenueService.Venues.Infrastructure.MessageBus.Consumers
//{
//    public class VenueRatingUpdatedConsumer : IConsumer<VenueRatingUpdatedEvent>
//    {
//        private readonly IVenueRepository _venueRepository;

//        // Dependency Injection ile Repository'i içeri alıyoruz
//        public VenueRatingUpdatedConsumer(IVenueRepository venueRepository)
//        {
//            _venueRepository = venueRepository;
//        }

//        public async Task Consume(ConsumeContext<VenueRatingUpdatedEvent> context)
//        {
//            // Repository üzerinden mekanı bul
//            var venue = await _venueRepository.GetByIdAsync(context.Message.VenueId);

//            if (venue != null)
//            {
//                // Değerleri güncelle
//                venue.AverageRating = context.Message.NewAverageRating;
//                venue.ReviewCount = context.Message.TotalReviewCount;

//                // Kaydet
//                await _venueRepository.UpdateAsync(venue);
//            }
//        }
//    }
//}

using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Review;
using MassTransit;
using Microsoft.Extensions.Logging; // Eklendi

namespace CityDiscovery.VenueService.Venues.Infrastructure.MessageBus.Consumers
{
    public class VenueRatingUpdatedConsumer : IConsumer<VenueRatingUpdatedEvent>
    {
        private readonly IVenueRepository _venueRepository;
        private readonly ILogger<VenueRatingUpdatedConsumer> _logger; // Eklendi

        public VenueRatingUpdatedConsumer(IVenueRepository venueRepository, ILogger<VenueRatingUpdatedConsumer> logger)
        {
            _venueRepository = venueRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<VenueRatingUpdatedEvent> context)
        {
            var venueId = context.Message.VenueId;

            // Log: "Mesaj geldi, işlem başlıyor"
            _logger.LogInformation($"Event alındı: VenueId={venueId} için puan güncelleniyor. Yeni Puan: {context.Message.NewAverageRating}");

            var venue = await _venueRepository.GetByIdAsync(venueId);

            if (venue != null)
            {
                venue.AverageRating = context.Message.NewAverageRating;
                venue.ReviewCount = context.Message.TotalReviewCount; // ReviewCount ismini Venue entity'nizdeki isimle eşleştirin

                await _venueRepository.UpdateAsync(venue);

                // Log: "Başarıyla bitti"
                _logger.LogInformation($"GÜNCELLEME BAŞARILI: {venue.Name} yeni puanı {venue.AverageRating} oldu.");
            }
            else
            {
                // Log: "Hata/Uyarı"
                _logger.LogWarning($"HATA: VenueId={venueId} veritabanında bulunamadı! Puan güncellenemedi.");
            }
        }
    }
}