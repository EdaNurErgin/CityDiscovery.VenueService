using CityDiscovery.Venues.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Configurations;

public class VenueAddressConfiguration : IEntityTypeConfiguration<VenueAddress>
{
    public void Configure(EntityTypeBuilder<VenueAddress> builder)
    {
        builder.ToTable("VenueAddresses");

        // Primary Key (İstersen Id, istersen direkt VenueId'yi PK yapabilirsin)
        builder.HasKey(x => x.Id);

        // Venue ile 1-1 İlişki
        builder.HasOne(x => x.Venue)
               .WithOne(v => v.Address)
               .HasForeignKey<VenueAddress>(x => x.VenueId)
               .OnDelete(DeleteBehavior.Cascade); // Mekan silinirse adresi de silinsin

        // Country, City ve District İlişkileri
        builder.HasOne(x => x.Country)
               .WithMany()
               .HasForeignKey(x => x.CountryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.City)
               .WithMany()
               .HasForeignKey(x => x.CityId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.District)
               .WithMany()
               .HasForeignKey(x => x.DistrictId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}