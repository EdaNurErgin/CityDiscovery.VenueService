using CityDiscovery.Venues.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CityDiscovery.Venues.Infrastructure.Data
{
    public class VenueDbContextFactory : IDesignTimeDbContextFactory<VenueDbContext>
    {
        public VenueDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

            var basePath = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connStr = config.GetConnectionString("DefaultConnection")
                         ?? throw new InvalidOperationException("Connection string 'DefaultConnection' bulunamadı.");

            var optionsBuilder = new DbContextOptionsBuilder<VenueDbContext>();

            optionsBuilder.UseSqlServer(
                connStr,
                sql =>
                {
                    sql.MigrationsAssembly(typeof(VenueDbContext).Assembly.FullName);
                    sql.UseNetTopologySuite(); 
                }
            );

            return new VenueDbContext(optionsBuilder.Options);
        }
    }
}
