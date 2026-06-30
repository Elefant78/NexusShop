using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusShop.Domain.Entities;

namespace NexusShop.Infrastructure.Persistence.Configurations;

/// <summary>Fluent API configuration for the Category entity.</summary>
public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.HasIndex(c => c.Name).IsUnique();

        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category!)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // EF needs access to the private backing field for the Products collection.
        builder.Metadata
            .FindNavigation(nameof(Category.Products))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
