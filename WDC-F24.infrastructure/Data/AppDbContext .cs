using Microsoft.EntityFrameworkCore;
using WDC_F24.infrastructure.Data;


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Text;
using WDC_F24.Domain.Entities;



namespace WDC_F24.infrastructure.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Order → OrderItem: One-to-Many
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId") // shadow FK
                .OnDelete(DeleteBehavior.Cascade);

            // Configure decimal precision for UnitPrice
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            // Optional: Indexes for performance
            modelBuilder.Entity<OrderItem>()
                .HasIndex(oi => oi.ProductId);
        }
    }
}
