using CityDiscovery.Venues.Infrastructure.MessageBus.Consumers;
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

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqConfig = configuration.GetSection("RabbitMq");

                cfg.Host(rabbitMqConfig["Host"], rabbitMqConfig["VirtualHost"], h =>
                {
                    h.Username(rabbitMqConfig["Username"] ?? "guest");
                    h.Password(rabbitMqConfig["Password"] ?? "guest");
                });

                // Consumer endpoints
                cfg.ReceiveEndpoint("venue-service-user-deleted", e =>
                {
                    e.ConfigureConsumer<UserDeletedEventConsumer>(context);
                });

                cfg.ReceiveEndpoint("venue-service-user-role-changed", e =>
                {
                    e.ConfigureConsumer<UserRoleChangedEventConsumer>(context);
                });
            });
        });

        return services;
    }
}