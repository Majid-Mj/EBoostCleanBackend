using EBoost.Application.Interfaces.Repositories;
using EBoost.Domain.Entities;
using EBoost.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EBoost.Infrastructure.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly EBoostDbContext _context;

    public WishlistRepository(EBoostDbContext context)
    {
        _context = context;
    }

    public async Task<Wishlist?> GetByUserIdAsync(int userId)
    {
        return await _context.Wishlists
            .Include(w => w.Items)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(w => w.UserId == userId);
    }

    public async Task AddItemAsync(int wishlistId, int productId)
    {
        var item = new WishlistItem
        {
            WishlistId = wishlistId,
            ProductId = productId
        };

        _context.WishlistItems.Add(item);   
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RemoveItemAsync(int wishlistId, int productId)
    {
        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(x =>
                x.WishlistId == wishlistId &&
                x.ProductId == productId);

        if (item == null)
            return false;

        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> ItemExistsAsync(int wishlistId, int productId)
    {
        return await _context.WishlistItems
            .AnyAsync(x =>
                x.WishlistId == wishlistId &&
                x.ProductId == productId);
    }

    public async Task<Wishlist> CreateAsync(int userId)
    {
        var wishlist = new Wishlist
        {
            UserId = userId
        };

        _context.Wishlists.Add(wishlist);
        await _context.SaveChangesAsync();

        return wishlist;
    }

}
