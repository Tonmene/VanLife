using Microsoft.EntityFrameworkCore;
using VanLife.Api.Models;

namespace VanLife.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Van> Vans => Set<Van>();
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<Seller> Sellers => Set<Seller>();
    public DbSet<Buyer> Buyers => Set<Buyer>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<ImageAsset> Images => Set<ImageAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rental>()
            .HasKey(r => r.PurchaseId);

        modelBuilder.Entity<UserAccount>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Van>()
            .HasOne(v => v.Seller)
            .WithMany(s => s.Vans)
            .HasForeignKey(v => v.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seller-Van relationship: Seller.SellerId -> Van.SellerId
        modelBuilder.Entity<Seller>()
            .HasMany(s => s.Vans)
            .WithOne(v => v.Seller)
            .HasForeignKey(v => v.SellerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Rental relationships: Rental.SellerId -> Seller.SellerId
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Seller)
            .WithMany(s => s.Rentals)
            .HasForeignKey(r => r.SellerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Rental relationships: Rental.BuyerId -> Buyer.BuyerId
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Buyer)
            .WithMany(b => b.Rentals)
            .HasForeignKey(r => r.BuyerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Rental relationships: Rental.VanId -> Van.Id (foreign key)
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Van)
            .WithMany()
            .HasForeignKey(r => r.VanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ImageAsset>()
            .HasOne(i => i.Van)
            .WithMany(v => v.Photos)
            .HasForeignKey(i => i.VanId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
