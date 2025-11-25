using CityDiscovery.Venues.Domain.Entities;
using CityDiscovery.Venues.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venuex>
{
    public void Configure(EntityTypeBuilder<Venuex> builder)
    {
        builder.ToTable("Venues");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.OwnerUserId)
            .IsRequired();

        builder.Property(v => v.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(v => v.Description)
            .HasMaxLength(1000);

        builder.Property(v => v.AddressText)
            .HasMaxLength(400);

        builder.Property(v => v.Phone)
            .HasMaxLength(50);

        builder.Property(v => v.WebsiteUrl)
            .HasMaxLength(300);

        builder.Property(v => v.OpeningHoursJson)
            .HasMaxLength(1000);

        // PriceLevel (Value Object) -> tinyint nullable
        builder.Property(v => v.PriceLevel)
            .HasConversion(
                pl => pl != null ? (byte?)pl.Value : null,
                value => value.HasValue ? PriceLevel.Create(value.Value) : null
            )
            .HasColumnType("tinyint");

        builder.Property(v => v.Location)
            .HasColumnType("geography")
            .IsRequired();

        builder.Property(v => v.IsApproved)
            .HasDefaultValue(false);

        builder.Property(v => v.IsActive)
            .HasDefaultValue(true);

        builder.Property(v => v.CreatedAt)
            .HasColumnType("datetime2");

        builder.Property(v => v.UpdatedAt)
            .HasColumnType("datetime2");

        // Owner başına tek venue
        builder.HasIndex(v => v.OwnerUserId)
            .IsUnique()
            .HasDatabaseName("UX_Venues_OwnerUser");

        // Onay/durum indexleri
        builder.HasIndex(v => new { v.IsApproved, v.IsActive })
            .HasDatabaseName("IX_Venues_IsApproved_IsActive");
    }
}
