using EBoost.Application.Interfaces.Repositories;
using EBoost.Infrastructure.Data;
using EBoost.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EBoost.Infrastructure.Repositories;

public class CartRepository : ICartRepository   
{
    private readonly EBoostDbContext _context;

    public CartRepository(EBoostDbContext context)
    {
        _context = context;
    }
    public async Task<Cart?> GetByUserIdAsync(int userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart> CreateAsync(int userId)
    {
        var cart = new Cart
        {
            UserId = userId
        };

        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        return cart;
    }

    public async Task<CartItem?> GetItemAsync(int cartId , int productId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(ci => 
                ci.CartId == cartId && ci.ProductId == productId);
    }

    public async Task AddItemAsync(CartItem item)
    {
        await _context.CartItems.AddAsync(item);
    }

    public async Task RemoveItemAsync (CartItem item)
    {
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
    }
    public async Task<int> ClearCartAsync(int cartId)
    {
        var items = await _context.CartItems
            .Where(ci => ci.CartId == cartId)
            .ToListAsync();

        if (!items.Any())
            return 0;

        _context.CartItems.RemoveRange(items);

        return items.Count;
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
 
}
