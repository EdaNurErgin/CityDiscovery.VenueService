using CityDiscovery.Venues.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityDiscovery.Venues.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        // DB PK: Id (int identity) -> Domain: CategoryId
        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.CategoryId)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        // Base Entity'den gelen Guid Id'yi DB'ye map etmiyoruz
        builder.Ignore(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Slug)
            .HasMaxLength(120)
            .IsRequired();

        builder.HasIndex(c => c.Slug)
            .IsUnique();

        builder.Property(c => c.IconUrl)
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        // Navigation: Category -> VenueCategories
        builder.HasMany(c => c.VenueCategories)
            .WithOne(vc => vc.Category)
            .HasForeignKey(vc => vc.CategoryId);
    }
}
