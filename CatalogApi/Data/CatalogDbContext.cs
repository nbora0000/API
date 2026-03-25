using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        });

        // Seed basic products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Description = "High performance laptop", Price = 1299.99m, Category = "Electronics", StockQuantity = 50 },
            new Product { Id = 2, Name = "Smartphone", Description = "Latest model smartphone", Price = 799.99m, Category = "Electronics", StockQuantity = 100 },
            new Product { Id = 3, Name = "Desk Chair", Description = "Ergonomic office chair", Price = 199.99m, Category = "Furniture", StockQuantity = 20 },
            new Product { Id = 4, Name = "Monitor", Description = "27 inch 4K monitor", Price = 349.99m, Category = "Electronics", StockQuantity = 30 }
        );
    }
}
