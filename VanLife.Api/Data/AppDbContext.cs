using Microsoft.EntityFrameworkCore;
using VanLife.Api.Models;

namespace VanLife.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Van> Vans => Set<Van>();
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<ImageAsset> Images => Set<ImageAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Van>()
            .HasOne(v => v.Seller)
            .WithMany(u => u.Vans)
            .HasForeignKey(v => v.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ImageAsset>()
            .HasOne(i => i.Van)
            .WithMany(v => v.Photos)
            .HasForeignKey(i => i.VanId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
