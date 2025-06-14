using kopinang_api.Models;
using Microsoft.EntityFrameworkCore;

namespace kopinang_api.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Produk> produk { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetail> order_detail { get; set; }
        public DbSet<Ulasan> ulasan { get; set; }  // Tambahan DbSet untuk ulasan

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: konfigurasi relasi & constraints
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Produk)
                .WithMany()
                .HasForeignKey(od => od.ProdukId);

            // Konfigurasi relasi Ulasan ke Order
            modelBuilder.Entity<Ulasan>()
                .HasOne(u => u.Order)
                .WithMany(o => o.Ulasan)  // Jika di model Order ada ICollection<Ulasan> Ulasan
                .HasForeignKey(u => u.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint user_id + order_id sudah ada di DB, bisa juga di EF:
            modelBuilder.Entity<Ulasan>()
                .HasIndex(u => new { u.OrderId, u.UserId })
                .IsUnique();
        }
    }
}
