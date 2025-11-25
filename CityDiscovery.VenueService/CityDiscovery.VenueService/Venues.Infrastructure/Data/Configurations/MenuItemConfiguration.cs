using CityDiscovery.Venues.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("MenuItems");

        builder.HasKey(mi => mi.Id);

        builder.Property(mi => mi.MenuCategoryId)
            .IsRequired();

        builder.Property(mi => mi.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(mi => mi.Description)
            .HasMaxLength(500);

        builder.Property(mi => mi.Price)
            .HasColumnType("decimal(10,2)");

        builder.Property(mi => mi.ImageUrl)
            .HasMaxLength(1000);

        builder.Property(mi => mi.IsAvailable)
            .HasDefaultValue(true);

        builder.Property(mi => mi.SortOrder)
            .HasDefaultValue(0);

        builder.Property(mi => mi.CreatedAt)
            .HasColumnType("datetime2");

        builder.Property(mi => mi.UpdatedAt)
            .HasColumnType("datetime2");

        // FK: MenuCategory
        builder.HasOne(mi => mi.MenuCategory)
            .WithMany(mc => mc.Items)
            .HasForeignKey(mi => mi.MenuCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index: (MenuCategoryId, SortOrder)
        builder.HasIndex(mi => new { mi.MenuCategoryId, mi.SortOrder })
            .HasDatabaseName("IX_MenuItems_Category_Sort");

        // Unique: MenuCategoryId + Name
        builder.HasIndex(mi => new { mi.MenuCategoryId, mi.Name })
            .IsUnique()
            .HasDatabaseName("UX_MenuItems_NamePerCategory");

        // Check constraint: Price >= 0 or NULL
        builder.HasCheckConstraint(
            "CK_MenuItems_PriceNonNegative",
            "[Price] IS NULL OR [Price] >= 0"
        );
    }
}
