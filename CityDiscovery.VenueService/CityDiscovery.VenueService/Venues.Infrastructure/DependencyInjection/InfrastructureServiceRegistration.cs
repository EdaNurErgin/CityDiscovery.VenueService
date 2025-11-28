using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using CityDiscovery.Venues.Infrastructure.Persistence.Repositories;
using CityDiscovery.Venues.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                    sql.UseNetTopologySuite(); // geography için önemli
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




        return services;
    }
}
