using BasketApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BasketApi.Data;

public class BasketDbContext : DbContext
{
    public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options) { }

    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
            entity.Ignore(e => e.TotalPrice);
            entity.HasMany(e => e.Items)
                  .WithOne()
                  .HasForeignKey("ShoppingCartCustomerId")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
        });
    }
}
