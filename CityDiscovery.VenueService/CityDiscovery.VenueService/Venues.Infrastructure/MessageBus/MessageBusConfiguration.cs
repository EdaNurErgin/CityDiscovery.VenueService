using CityDiscovery.Venues.Infrastructure.MessageBus.Consumers;
using CityDiscovery.VenueService.Venues.Infrastructure.MessageBus.Consumers;
using MassTransit;


namespace CityDiscovery.Venues.Infrastructure.MessageBus;

public static class MessageBusConfiguration
{
    public static IServiceCollection AddVenueMessageBus(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Consumers
            x.AddConsumer<UserDeletedEventConsumer>();
            x.AddConsumer<UserRoleChangedEventConsumer>();
            x.AddConsumer<VenueRatingUpdatedConsumer>();
            x.AddConsumer<ContentRemovedConsumer>();

            x.SetKebabCaseEndpointNameFormatter(); // İsimlendirme standardı

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqConfig = configuration.GetSection("RabbitMq");
                var vHost = rabbitMqConfig["VirtualHost"];
                if (string.IsNullOrEmpty(vHost)) vHost = "/";

                cfg.Host(rabbitMqConfig["Host"], vHost, h =>
                {
                    h.Username(rabbitMqConfig["Username"] ?? "guest");
                    h.Password(rabbitMqConfig["Password"] ?? "guest");
                });

                

                // User Silindiğinde:
                cfg.ReceiveEndpoint("venue-service-user-deleted", e =>
                {
                    // Identity'nin exchange ismine zorla bağlıyoruz.
                    e.Bind("IdentityService.Shared.MessageBus.Identity:UserDeletedEvent");
                    e.ConfigureConsumer<UserDeletedEventConsumer>(context);
                });

                // Rol Değiştiğinde:
                cfg.ReceiveEndpoint("venue-service-user-role-changed", e =>
                {
                    // Bunu da Identity'ye bağlamak iyi olur
                    e.Bind("IdentityService.Shared.MessageBus.Identity:UserRoleChangedEvent");
                    e.ConfigureConsumer<UserRoleChangedEventConsumer>(context);
                });

                // --- 2. Kendi İçindeki Olaylar (Bind şart değil ama manuel isim verdin) ---

                cfg.ReceiveEndpoint("venue-service-rating-updated", e =>
                {
                    e.ConfigureConsumer<VenueRatingUpdatedConsumer>(context);
                });

                cfg.ReceiveEndpoint("content-removed-venue-queue", e =>
                {
                    e.ConfigureConsumer<ContentRemovedConsumer>(context);
                });

                // ---  Geriye Kalan Her Şey İçin Otomatik Yapılandırma ---
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}