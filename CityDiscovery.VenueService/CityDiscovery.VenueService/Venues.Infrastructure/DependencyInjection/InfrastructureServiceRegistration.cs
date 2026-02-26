using CityDiscovery.Venues.Application.Interfaces.ExternalServices;
using CityDiscovery.Venues.Application.Interfaces.MessageBus;
using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using CityDiscovery.Venues.Infrastructure.ExternalServices;
using CityDiscovery.Venues.Infrastructure.MessageBus;
using CityDiscovery.Venues.Infrastructure.Persistence.Repositories;
using CityDiscovery.Venues.Infrastructure.Persistence.Repository;
using CityDiscovery.VenueService.Venues.Infrastructure.Services;
using CityDiscovery.VenueService.VenuesService.Application.Interfaces.Services;
using CityDiscovery.VenueService.VenuesService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;


namespace CityDiscovery.Venues.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddVenueInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<VenueDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(typeof(VenueDbContext).Assembly.FullName);
                    sql.UseNetTopologySuite();
                    sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
        });

        // Repositories
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IVenueCategoryRepository, VenueCategoryRepository>();
        services.AddScoped<IVenuePhotoRepository, VenuePhotoRepository>();
        services.AddScoped<IMenuCategoryRepository, MenuCategoryRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IVenueSearchService, VenueSearchService>();
        services.AddScoped<ICityDiscoveryService, CityDiscoveryElasticService>();

        // External Services (HTTP Clients)
        services.AddHttpClient<IIdentityServiceClient, IdentityServiceClient>();

        // Message Bus
        services.AddVenueMessageBus(configuration);
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        return services;
    }
}
