using EBoost.Application.DTOs.Order;
using EBoost.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Interfaces.Services;

public interface IOrderService
{
    Task PlaceOrderFromCartAsync(int userId,
    int? addressId,
    PaymentMethod paymentMethod);
    Task BuyNowAsync(int userId, int productId, int quantity, int? addressId, PaymentMethod paymentMethod);
    Task<List<OrderDto>> GetMyOrdersAsync(int userId);
    Task<OrderDto?> GetByIdAsync(int id, int userId);
    Task<OrderDto?> CancelOrderAsync(int orderId, int userId);

    Task<(List<OrderDto> Orders, int TotalCount)> GetAllOrdersAsync(
    int page,
    int pageSize,
    string? status = null);

    Task<OrderDto?> UpdateOrderStatusAsync(
    int orderId,
    string newStatus);

    Task ConfirmPaymentAsync(int orderId, int userId, string transactionId);




}
