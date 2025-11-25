using CityDiscovery.Venues.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Configurations;

public class VenuePhotoConfiguration : IEntityTypeConfiguration<VenuePhoto>
{
    public void Configure(EntityTypeBuilder<VenuePhoto> builder)
    {
        builder.ToTable("VenuePhotos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Url)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(p => p.Caption)
            .HasMaxLength(200);

        builder.Property(p => p.SortOrder)
            .HasDefaultValue(0);

        builder.Property(p => p.CreatedAt)
            .HasColumnType("datetime2");

        builder.HasOne(p => p.Venue)
            .WithMany(v => v.Photos)
            .HasForeignKey(p => p.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.VenueId, p.SortOrder })
            .HasDatabaseName("IX_VenuePhotos_Venue_Sort");
    }
}
