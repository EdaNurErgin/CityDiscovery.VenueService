using CityDiscovery.Venues.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Configurations;

public class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
{
    public void Configure(EntityTypeBuilder<MenuCategory> builder)
    {
        builder.ToTable("MenuCategories");

        // MenuCategoryId -> Id (INT IDENTITY)
        builder.HasKey(mc => mc.MenuCategoryId);

        builder.Property(mc => mc.MenuCategoryId)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        // Base Entity Guid Id'yi map etme
        builder.Ignore(mc => mc.Id);

        builder.Property(mc => mc.VenueId)
            .IsRequired();

        builder.Property(mc => mc.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(mc => mc.SortOrder)
            .HasDefaultValue(0);

        builder.Property(mc => mc.IsActive)
            .HasDefaultValue(true);

        builder.Property(mc => mc.CreatedAt)
            .HasColumnType("datetime2");

        builder.Property(mc => mc.UpdatedAt)
            .HasColumnType("datetime2");

        builder.HasOne(mc => mc.Venue)
            .WithMany(v => v.MenuCategories)
            .HasForeignKey(mc => mc.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(mc => new { mc.VenueId, mc.SortOrder })
            .HasDatabaseName("IX_MenuCategories_Venue_Sort");
    }
}
