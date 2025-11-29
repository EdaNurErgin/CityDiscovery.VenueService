//using CityDiscovery.Venues.Application.Interfaces.Repositories;
//using CityDiscovery.Venues.Domain.Entities;
//using CityDiscovery.Venues.Domain.ValueObjects;
//using MediatR;

//namespace CityDiscovery.Venues.Application.Features.Venues.Commands.CreateVenue
//{
//    public sealed class CreateVenueCommandHandler
//        : IRequestHandler<CreateVenueCommand, Guid>
//    {
//        private readonly IVenueRepository _venueRepository;

//        public CreateVenueCommandHandler(
//            IVenueRepository venueRepository)
//        {
//            _venueRepository = venueRepository;
//        }


//        public async Task<Guid> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
//        {
//            // 1. Owner zaten bir mekana sahip mi?
//            if (await _venueRepository.OwnerHasVenueAsync(request.OwnerUserId, cancellationToken))
//                throw new InvalidOperationException("Owner already has a venue.");

//            // 2. PriceLevel value object
//            var price = PriceLevel.FromNullable(request.PriceLevel);

//            // 3. Domain entity creation (GeoLocation yok, direkt lat/lon)
//            var venue = Venuex.Create(
//                request.OwnerUserId,
//                request.Name,
//                request.Description,
//                request.AddressText,
//                request.Phone,
//                request.WebsiteUrl,
//                price,
//                request.OpeningHoursJson,
//                request.Latitude,   // ✅
//                request.Longitude   // ✅
//            );

//            // 4. Persist
//            await _venueRepository.AddAsync(venue, cancellationToken);

//            return venue.Id;
//        }
//    }
//}
using CityDiscovery.Venues.Application.Interfaces.ExternalServices;
using CityDiscovery.Venues.Application.Interfaces.MessageBus;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Domain.ValueObjects;
using CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CityDiscovery.Venues.Application.Features.Venues.Commands.CreateVenue;

public sealed class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, Guid>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IIdentityServiceClient _identityServiceClient;
    private readonly ILogger<CreateVenueCommandHandler> _logger;

    public CreateVenueCommandHandler(
        IVenueRepository venueRepository,
        IEventPublisher eventPublisher,
        IIdentityServiceClient identityServiceClient,
        ILogger<CreateVenueCommandHandler> logger)
    {
        _venueRepository = venueRepository;
        _eventPublisher = eventPublisher;
        _identityServiceClient = identityServiceClient;
        _logger = logger;
    }

    public async Task<Guid> Handle(
        CreateVenueCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Identity Service'den kullanıcı kontrolü (Owner rolünde mi?)
        // NOT: Bu kontrol opsiyonel. Eğer Identity Service çalışmıyorsa veya user bulunamazsa,
        // sadece log'la ama exception fırlatma. Çünkü zaten token'dan Owner kontrolü yapılıyor.
        // Identity Service kontrolü sadece ekstra güvenlik için.
        try
        {
            var user = await _identityServiceClient.GetUserAsync(request.OwnerUserId, cancellationToken);
            if (user == null)
            {
                // Identity Service'de user bulunamadı ama token'dan Owner kontrolü yapıldı
                // Bu durumda devam edebiliriz, sadece uyarı ver
                _logger.LogWarning("User {UserId} not found in Identity Service, but token validation passed. Proceeding with venue creation.", 
                    request.OwnerUserId);
            }
            else
            {
                // User bulundu, rol kontrolü yap
                if (!await _identityServiceClient.CheckUserRoleAsync(request.OwnerUserId, "Owner", cancellationToken))
                {
                    // Identity Service'de user Owner değil ama token'dan Owner kontrolü yapıldı
                    // Bu durumda devam edebiliriz, sadece uyarı ver
                    _logger.LogWarning("User {UserId} is not Owner in Identity Service, but token validation passed. Proceeding with venue creation.", 
                        request.OwnerUserId);
                }
            }
        }
        catch (Exception ex)
        {
            // Identity Service bağlantı hatası - devam et, token kontrolü yeterli
            _logger.LogWarning(ex, "Identity Service check failed for user {UserId}, but token validation passed. Proceeding with venue creation.", 
                request.OwnerUserId);
        }

        // 2. Owner zaten bir mekana sahip mi?
        if (await _venueRepository.OwnerHasVenueAsync(request.OwnerUserId, cancellationToken))
            throw new InvalidOperationException("Owner already has a venue.");

        // 3. PriceLevel value object
        var price = PriceLevel.FromNullable(request.PriceLevel);

        // 4. Domain entity creation
        var venue = Venuex.Create(
            request.OwnerUserId,
            request.Name,
            request.Description,
            request.AddressText,
            request.Phone,
            request.WebsiteUrl,
            price,
            request.OpeningHoursJson,
            request.Latitude,
            request.Longitude
        );

        // 5. Persist (Tüm kontroller geçildikten sonra kaydet)
        await _venueRepository.AddAsync(venue, cancellationToken);

        // 6. Event yayınla (Admin'e bildirim için)
        // Event yayınlama hatası olsa bile veri kaydedilmiş olmalı, bu yüzden try-catch ile sarmalayalım
        try
        {
            await _eventPublisher.PublishAsync(new VenueCreatedEvent
            {
                VenueId = venue.Id,
                OwnerUserId = venue.OwnerUserId,
                Name = venue.Name,
                IsApproved = venue.IsApproved,
                CreatedAt = venue.CreatedAt
            }, cancellationToken);
        }
        catch (Exception)
        {
            // Event yayınlama hatası olsa bile venue kaydedilmiş, log'la ama exception fırlatma
            // Çünkü veri tutarlılığı önemli, event sonra tekrar gönderilebilir
        }

        return venue.Id;
    }
}
