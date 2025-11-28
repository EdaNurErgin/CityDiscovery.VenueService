using CityDiscovery.Venues.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.Venues.Infrastructure.Data.Context;

public class VenueDbContext : DbContext
{
    public VenueDbContext(DbContextOptions<VenueDbContext> options)
        : base(options)
    {
    }

    public DbSet<Venuex> Venues => Set<Venuex>();
    public DbSet<VenueAddress> VenueAddresses => Set<VenueAddress>();
    public DbSet<VenueCategory> VenueCategories => Set<VenueCategory>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<VenuePhoto> VenuePhotos => Set<VenuePhoto>();
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Event> Events => Set<Event>();





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VenueDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
