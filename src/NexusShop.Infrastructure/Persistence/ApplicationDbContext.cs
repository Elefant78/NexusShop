using Microsoft.EntityFrameworkCore;
using NexusShop.Domain.Entities;

namespace NexusShop.Infrastructure.Persistence;

/// <summary>
/// The EF Core database context. Uses the Code-First approach: the schema is
/// derived from the entity classes and the Fluent API configurations found in
/// this assembly.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Picks up every IEntityTypeConfiguration<T> in this assembly
        // (ProductConfiguration, CategoryConfiguration, ...).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
