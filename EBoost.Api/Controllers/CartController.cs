using AutoMapper;
using EBoost.Api.Extensions;
using EBoost.Application.DTOs.Cart;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Application.Interfaces.Services;
using EBoost.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EBoost.Api.Controllers;


[Authorize(Policy = "UserOnly")]
[Route("api/Cart")]
[ApiController]

public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepo;
    private readonly ICartService _cartService;
    private readonly IMapper _mapper;

    public CartController(ICartService cartService, IMapper mapper , ICartRepository cartRepo)
    {
        _cartService = cartService;
        _mapper = mapper;
        _cartRepo = cartRepo;
    }

    [HttpPost("{productId:int}")]
    public async Task<IActionResult> Add(int productId)
    {
        int userId = User.GetUserId();

        await _cartService.AddToCartAsync(userId, productId);

        return Ok("Added to cart");
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        int userId = User.GetUserId();

        var cart = await _cartRepo.GetByUserIdAsync(userId);

        if (cart == null)
            return Ok(new CartDto());

        var cartDto = _mapper.Map<CartDto>(cart);

        return Ok(cartDto);
    }


    [HttpPut("update")]
    public async Task<IActionResult> Update([FromForm] UpdateCartDto dto)
    {
        int userId = User.GetUserId();

        await _cartService.UpdateQuantityAsync(userId, dto.ProductId, dto.Quantity);

        return Ok("Cart updated");
    }


    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Remove(int productId)
    {
        int userId = User.GetUserId();

        await _cartService.RemoveFromCartAsync(userId, productId);

        return Ok("Product removed from cart");
    }
}
