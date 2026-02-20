

namespace EBoost.Application.Interfaces.Services;

public interface ICartService
{
    Task AddToCartAsync(int userId, int productId);
    Task GetCartAsync(int userId);
    Task RemoveFromCartAsync(int userId, int productId);
    Task UpdateQuantityAsync(int userId, int productId, int quantity);
}

