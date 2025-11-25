using CityDiscovery.Venues.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.VenueId)
            .IsRequired();

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .HasColumnType("datetime2");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("datetime2");

        builder.HasOne(e => e.Venue)
            .WithMany(v => v.Events)
            .HasForeignKey(e => e.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index: (VenueId, StartDate DESC)
        builder.HasIndex(e => new { e.VenueId, e.StartDate })
            .HasDatabaseName("IX_Events_Venue_StartDate");

        // Index: (StartDate, IsActive) WHERE IsActive = 1
        builder.HasIndex(e => new { e.StartDate, e.IsActive })
            .HasDatabaseName("IX_Events_StartDate_Active")
            .HasFilter("[IsActive] = 1");

        // Check constraint: EndDate >= StartDate or NULL
        builder.HasCheckConstraint(
            "CK_Events_DateRange",
            "[EndDate] IS NULL OR [EndDate] >= [StartDate]"
        );
    }
}
