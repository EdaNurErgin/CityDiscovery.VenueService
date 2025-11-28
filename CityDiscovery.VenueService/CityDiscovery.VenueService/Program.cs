using CityDiscovery.Venues.Application.DependencyInjection;
using CityDiscovery.Venues.Application.Interfaces.Services;
using CityDiscovery.Venues.Infrastructure.DependencyInjection;
using CityDiscovery.Venues.Infrastructure.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders;

namespace CityDiscovery.VenueService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<LocalFileStorageOptions>(
                builder.Configuration.GetSection("LocalFileStorage"));
            builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

            builder.Services.AddVenueApplication();
            // Add services to the container.
            builder.Services.AddVenueInfrastructure(builder.Configuration);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
        builder.Configuration["LocalFileStorage:RootPath"]!),
                RequestPath = "/uploads"
            });


            app.MapControllers();

            app.Run();
        }
    }
}
