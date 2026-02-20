using AutoMapper;
using EBoost.Application.DTOs.Cart;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Application.Interfaces.Services;
using EBoost.Domain.Entities;

namespace EBoost.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository cartRepo;
    private readonly IProductRepository productRepo;
    private readonly IMapper mapper;

    public CartService(ICartRepository cartRepo, IProductRepository productRepo , IMapper mapper)
    {
        this.cartRepo = cartRepo;
        this.productRepo = productRepo;
        this.mapper = mapper;
    }

    public async Task AddToCartAsync(int userId, int productId)
    {
        var product = await productRepo.GetByIdAsync(productId);

        if (product == null || !product.IsActive)
            throw new Exception("Product not found");

        if (product.Stock <= 0)
            throw new Exception("Product out of stock");

        var cart = await cartRepo.GetByUserIdAsync(userId);

        if (cart == null)
            cart = await cartRepo.CreateAsync(userId);

        var existingItem = await cartRepo.GetItemAsync(cart.Id, productId);

        if (existingItem != null)
        {
            if (existingItem.Quantity >= product.Stock)
                throw new Exception("Not enough stock");

            existingItem.Quantity += 1;
        }
        else
        {
            await cartRepo.AddItemAsync(new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = 1
            });
        }

        await cartRepo.SaveChangesAsync();
    }


    public async Task UpdateQuantityAsync(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new Exception("Quantity must be greater than zero");

        var cart = await cartRepo.GetByUserIdAsync(userId);

        if (cart == null)
            throw new Exception("Cart not found");

        var item = await cartRepo.GetItemAsync(cart.Id, productId);

        if (item == null)
            throw new Exception("Product not found in cart");

        var product = await productRepo.GetByIdAsync(productId);

        if (product == null || quantity > product.Stock)
            throw new Exception("Invalid quantity");

        item.Quantity = quantity;

        await cartRepo.SaveChangesAsync();
    }


    public async Task RemoveFromCartAsync(int userId, int productId)
    {
        var cart = await cartRepo.GetByUserIdAsync(userId);

        if (cart == null)
            throw new Exception("Cart not found");

        var item = await cartRepo.GetItemAsync(cart.Id, productId);

        if (item == null)
            throw new Exception("Product not found in cart");

        await cartRepo.RemoveItemAsync(item);
        await cartRepo.SaveChangesAsync();
    }

    public Task GetCartAsync(int userId)
    {
        throw new NotImplementedException();
    }
}
