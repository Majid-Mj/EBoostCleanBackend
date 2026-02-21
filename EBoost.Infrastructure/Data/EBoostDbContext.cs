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
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        //Fluent Configuration for wislist 
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

        modelBuilder.Entity<Category>()
        .HasIndex(c => c.Name)
        .IsUnique();

        modelBuilder.Entity<Product>()
        .HasIndex(p => new { p.Name, p.CategoryId })
        .IsUnique();

        //ShippingAddress
        modelBuilder.Entity<ShippingAddress>()
             .Property(a => a.FullName)
             .HasMaxLength(100)
             .IsRequired();


    }
}

