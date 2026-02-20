

using EBoost.Domain.Entities;

namespace EBoost.Application.Interfaces.Repositories;

public interface IWishlistRepository
{
    Task<Wishlist?> GetByUserIdAsync(int UserId);
    Task AddItemAsync(int wishlistId, int productId);
    Task<bool> RemoveItemAsync(int wishlistId, int productId);
    Task<bool> ItemExistsAsync(int id, int productId);
    Task<Wishlist> CreateAsync(int userId);

}
