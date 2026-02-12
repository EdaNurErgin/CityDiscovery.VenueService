using CityDiscovery.Venues.Infrastructure.MessageBus.Consumers;
using CityDiscovery.VenueService.Venues.Infrastructure.MessageBus.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqConfig = configuration.GetSection("RabbitMq");

                // VirtualHost ayarına dikkat! Eğer boş gelirse "/" kullanılmalı.
                var vHost = rabbitMqConfig["VirtualHost"];
                if (string.IsNullOrEmpty(vHost)) vHost = "/";

                cfg.Host(rabbitMqConfig["Host"], vHost, h =>
                {
                    h.Username(rabbitMqConfig["Username"] ?? "guest");
                    h.Password(rabbitMqConfig["Password"] ?? "guest");
                });

                // --- ÖNEMLİ EKLENTİ: 1 ---
                // Manuel tanımladıklarınızın dışında kalan her şeyi (Publisher ayarları dahil) otomatik yapılandırır.
                cfg.ConfigureEndpoints(context);
                // --------------------------

                // Manuel endpoint tanımlarınız (Bunlar kalabilir, ConfigureEndpoints bunları ezmez)
                cfg.ReceiveEndpoint("venue-service-user-deleted", e =>
                {
                    e.ConfigureConsumer<UserDeletedEventConsumer>(context);
                });

                cfg.ReceiveEndpoint("venue-service-user-role-changed", e =>
                {
                    e.ConfigureConsumer<UserRoleChangedEventConsumer>(context);
                });

                cfg.ReceiveEndpoint("venue-service-rating-updated", e =>
                {
                    e.ConfigureConsumer<VenueRatingUpdatedConsumer>(context);
                });
                cfg.ReceiveEndpoint("content-removed-venue-queue", e =>
                {
                    e.ConfigureConsumer<ContentRemovedConsumer>(context);
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}