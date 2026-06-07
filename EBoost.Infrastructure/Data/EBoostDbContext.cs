using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBoost.Domain.Entities;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.Serialization.Formatters;

namespace EBoost.Infrastructure.Data;

public class EBoostDbContext : DbContext
{
    public EBoostDbContext(DbContextOptions<EBoostDbContext> options)
        :base(options)
        {

        }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();    
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ShippingAddress> ShippingAddresses { get; set; } = null!;
    public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Entity Configurations from Assembly (Product, Category, ProductImage)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EBoostDbContext).Assembly);

        // Configure User constraints and index
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(256);

            entity.Property(u => u.PasswordHash)
                  .IsRequired()
                  .HasMaxLength(256);
        });

        // Configure RefreshToken index and constraints
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(t => t.TokenHash);
            entity.Property(t => t.TokenHash)
                  .IsRequired()
                  .HasMaxLength(128);
        });

        // Configure PasswordResetOtp index and constraints
        modelBuilder.Entity<PasswordResetOtp>(entity =>
        {
            entity.HasIndex(o => o.Email);
            entity.Property(o => o.Email)
                  .IsRequired()
                  .HasMaxLength(256);
            entity.Property(o => o.OtpHash)
                  .IsRequired()
                  .HasMaxLength(128);
        });

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        //Fluent Configuration for wishlist 
        //for prevent the duplicate Product and one wishlist per user;
        modelBuilder.Entity<Wishlist>()
            .HasIndex(w => w.UserId)
            .IsUnique();

        modelBuilder.Entity<WishlistItem>()
            .HasIndex(wi => new { wi.WishlistId , wi.ProductId})
            .IsUnique();

        //for the Cart
        modelBuilder.Entity<Cart>()
            .HasIndex(c => c.UserId)
            .IsUnique();

        modelBuilder.Entity<CartItem>()
            .HasIndex(ci => new { ci.CartId, ci.ProductId })
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.Name, p.CategoryId })
            .IsUnique();

        // Configure decimal precision for financial/monetary fields
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
            entity.Property(o => o.SubTotal).HasPrecision(18, 2);
            entity.Property(o => o.ShippingCost).HasPrecision(18, 2);
            entity.Property(o => o.GrandTotal).HasPrecision(18, 2);
            entity.Property(o => o.GrandCost).HasPrecision(18, 2);
        });

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);
    }
}

