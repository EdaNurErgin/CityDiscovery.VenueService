using CityDiscovery.Venues.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");
        builder.Ignore(x => x.Id); // Base Guid Id'yi yoksay
        builder.HasKey(x => x.CountryId);
        builder.Property(x => x.CountryId).HasColumnName("Id").ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(3);
    }
}

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");
        builder.Ignore(x => x.Id);
        builder.HasKey(x => x.CityId);
        builder.Property(x => x.CityId).HasColumnName("Id").ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

        builder.HasOne(x => x.Country)
            .WithMany(c => c.Cities)
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Restrict); // Ülke silinirse şehirler silinmesin (genelde)
    }
}

public class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.ToTable("Districts");
        builder.Ignore(x => x.Id);
        builder.HasKey(x => x.DistrictId);
        builder.Property(x => x.DistrictId).HasColumnName("Id").ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

        builder.HasOne(x => x.City)
            .WithMany(c => c.Districts)
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}