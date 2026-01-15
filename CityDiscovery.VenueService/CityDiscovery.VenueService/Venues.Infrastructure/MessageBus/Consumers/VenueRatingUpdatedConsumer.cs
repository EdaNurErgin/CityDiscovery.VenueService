using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Review;
using MassTransit;

namespace CityDiscovery.VenueService.Venues.Infrastructure.MessageBus.Consumers
{
    public class VenueRatingUpdatedConsumer : IConsumer<VenueRatingUpdatedEvent>
    {
        private readonly IVenueRepository _venueRepository;

        // Dependency Injection ile Repository'i içeri alıyoruz
        public VenueRatingUpdatedConsumer(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task Consume(ConsumeContext<VenueRatingUpdatedEvent> context)
        {
            // Repository üzerinden mekanı bul
            var venue = await _venueRepository.GetByIdAsync(context.Message.VenueId);

            if (venue != null)
            {
                // Değerleri güncelle
                venue.AverageRating = context.Message.NewAverageRating;
                venue.ReviewCount = context.Message.TotalReviewCount;

                // Kaydet
                await _venueRepository.UpdateAsync(venue);
            }
        }
    }
}