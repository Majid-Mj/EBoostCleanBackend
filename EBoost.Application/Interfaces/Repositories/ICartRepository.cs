using EBoost.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart> CreateAsync(int userId);
    Task<CartItem?> GetItemAsync(int cartId, int productId);
    Task AddItemAsync(CartItem item);
    Task RemoveItemAsync(CartItem item);
    Task SaveChangesAsync();
    Task<int> ClearCartAsync(int cartId);

}

