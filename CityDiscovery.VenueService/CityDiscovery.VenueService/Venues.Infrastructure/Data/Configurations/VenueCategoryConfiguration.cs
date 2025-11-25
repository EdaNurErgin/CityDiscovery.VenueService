using CityDiscovery.Venues.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Configurations;

public sealed class VenueCategoryConfiguration : IEntityTypeConfiguration<VenueCategory>
{
    public void Configure(EntityTypeBuilder<VenueCategory> builder)
    {
        builder.ToTable("VenueCategories");

        // 🔹 PK: (VenueId, CategoryId)
        builder.HasKey(vc => new { vc.VenueId, vc.CategoryId });

        // 🔹 Base Entity'deki Guid Id'yi map ETME
        builder.Ignore(vc => vc.Id);

        builder.Property(vc => vc.VenueId)
            .IsRequired();

        builder.Property(vc => vc.CategoryId)
            .IsRequired();

        builder.HasOne(vc => vc.Venue)
            .WithMany(v => v.VenueCategories)
            .HasForeignKey(vc => vc.VenueId);

        builder.HasOne(vc => vc.Category)
            .WithMany(c => c.VenueCategories)
            .HasForeignKey(vc => vc.CategoryId);
    }
}
