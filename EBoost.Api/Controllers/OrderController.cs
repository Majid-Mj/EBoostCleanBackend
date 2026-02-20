using EBoost.Api.Extensions;
using EBoost.Application.Interfaces.Services;
using EBoost.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EBoost.Application.DTOs.Order;

namespace EBoost.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }


    //[HttpPost("add-from-cart")]
    //public async Task<IActionResult> AddFromCart()
    //{
    //    int userId = User.GetUserId();
    //    await _orderService.PlaceOrderFromCartAsync(userId);
    //    return Ok("Order placed successfully");
    //}

    [HttpPost("add-from-cart")]
    public async Task<IActionResult> AddFromCart([FromForm]CartCheckoutDto dto)
    {
        int userId = User.GetUserId();

        await _orderService.PlaceOrderFromCartAsync(
            userId,
            dto.AddressId,
            dto.PaymentMethod);

        return Ok("Order placed successfully");
    }


    //[HttpPost("buy-now")]
    //public async Task<IActionResult> BuyNow(int productId , int quantity)
    //{
    //    int userId = User.GetUserId();
    //    await _orderService.BuyNowAsync(userId, productId, quantity);
    //    return Ok("Order placed successfully ");
    //}

    [HttpPost("buy-now")]
    public async Task<IActionResult> BuyNow([FromForm]BuyNowCheckoutDto dto)
    {
        int userId = User.GetUserId();

        await _orderService.BuyNowAsync(
            userId,
            dto.ProductId,
            dto.Quantity,
            dto.AddressId,
            dto.PaymentMethod);

        return Ok("Order placed successfully");
    }


    //my Orders
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        int userId = User.GetUserId();
        var orders = await _orderService.GetMyOrdersAsync(userId);
        return Ok(orders);
    }

    //Get OrderbyID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        int userId = User.GetUserId();
        var order = await _orderService.GetByIdAsync(id, userId);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    //Patch for cancel Order
    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        int userId = User.GetUserId();

        var result = await _orderService.CancelOrderAsync(id, userId);

        if (result == null)
            return NotFound("Order not found");

        return Ok(result);
    }

    // Get All Orders for admin
    [Authorize(Policy = "AdminOnly")]
    [HttpGet("admin")]
    public async Task<IActionResult> GetAllOrders(int page, int pagesize , string? Status = null)
    {
        var (orders , totalCount) = 
            await _orderService.GetAllOrdersAsync(page, pagesize, Status);

        return Ok(new
        {
            ToatalCount = totalCount,
            Page = page,
            PageSize = pagesize,
            Data = orders
        });
            
    }


    //Update the Status
    [Authorize(Policy = "AdminOnly")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
    int id,
    [FromQuery] string status)
    {
        var result =
            await _orderService.UpdateOrderStatusAsync(id, status);

        if (result == null)
            return NotFound("Order not found");

        return Ok(result);
    }

     
}
