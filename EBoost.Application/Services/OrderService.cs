using AutoMapper;
using EBoost.Application.DTOs.Order;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Application.Interfaces.Services;
using EBoost.Domain.Entities;
using EBoost.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EBoost.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly ICartRepository _cartRepo;
    private readonly IProductRepository _productRepo;
    private readonly IShippingAddressRepository _addressRepo;
    private readonly IMapper _mapper;

    public OrderService(
        IOrderRepository orderRepo,
        ICartRepository cartRepo,
        IProductRepository productRepo,
        IShippingAddressRepository addressRepo,
        IMapper mapper)
    {
        _orderRepo = orderRepo;
        _cartRepo = cartRepo;
        _productRepo = productRepo;
        _addressRepo = addressRepo;
        _mapper = mapper;
    }

    //order from cart
        public async Task PlaceOrderFromCartAsync(int userId,
        int? addressId,
        PaymentMethod paymentMethod)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId);

            if (cart == null || cart.Items == null || !cart.Items.Any())
                throw new Exception("Cart is empty");

        ShippingAddress address;

        if (addressId.HasValue && addressId.Value > 0)
        {
            address = await _addressRepo.GetByIdAsync(addressId.Value);

            if (address == null || address.UserId != userId)
                throw new Exception("Invalid shipping address");
        }
        else
        {
            address = await _addressRepo.GetDefaultByUserIdAsync(userId)
                ?? throw new Exception("Please set a default shipping address.");
        }

        //getting default address
        //var address = await _addressRepo.GetDefaultByUserIdAsync(userId);


            if (address == null)
                throw new Exception("Please set a default shipping address before placing order.");


            await _orderRepo.ExecuteInTransactionAsync(async () =>
            {
                var order = new Order
                {
                    UserId = userId,
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow,
                };

                _mapper.Map(address, order);

                foreach (var cartItem in cart.Items)
                {
                    var product = await _productRepo.GetByIdAsync(cartItem.ProductId);

                    if (product == null || !product.IsActive)
                        throw new Exception("Invalid product");

                    if (cartItem.Quantity > product.Stock)
                        throw new Exception("Insufficient stock");

                    product.Stock -= cartItem.Quantity;

                    order.Items.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = cartItem.Quantity
                    });
                }

                order.TotalAmount = order.Items
                    .Sum(i => i.UnitPrice * i.Quantity);

                await _orderRepo.AddAsync(order);

                object value = await _cartRepo.ClearCartAsync(cart.Id);
            });
        }  

        //Buy now 
        public async Task BuyNowAsync(int userId, int productId, int quantity ,  int? addressId, PaymentMethod paymentMethod)
        {
            var product = await _productRepo.GetByIdAsync(productId);

            if (product == null || !product.IsActive)
                throw new Exception("Invalid product");

            if (quantity <= 0)
                throw new Exception("Invalid quantity");

            if (quantity > product.Stock)
                throw new Exception("Insufficient stock");

            //var address = await _addressRepo.GetDefaultByUserIdAsync(userId);
            ShippingAddress address;

            if (addressId.HasValue)
            {
                address = await _addressRepo.GetByIdAsync(addressId.Value);

                if (address == null || address.UserId != userId)
                    throw new Exception("Invalid shipping address");
            }
            else
            {
                address = await _addressRepo.GetDefaultByUserIdAsync(userId)
                    ?? throw new Exception("Please set a default shipping address.");
            }



            await _orderRepo.ExecuteInTransactionAsync(async () =>
            {
                product.Stock -= quantity;

                var subTotal = quantity * product.Price;
                var shippingCost = 50; // example flat rate
                var grandTotal = subTotal + shippingCost;

                var order = new Order
                {
                    UserId = userId,
                    Status = paymentMethod == PaymentMethod.CashOnDelivery
                        ? OrderStatus.Confirmed
                        : OrderStatus.Pending,

                    PaymentMethod = paymentMethod,
                    PaymentStatus = paymentMethod == PaymentMethod.CashOnDelivery
                        ? PaymentStatus.Pending
                        : PaymentStatus.Pending,

                    OrderDate = DateTime.UtcNow,
                    SubTotal = subTotal,
                    ShippingCost = shippingCost,
                    GrandTotal = grandTotal
                };


                _mapper.Map(address, order);


                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = quantity
                });

                order.TotalAmount = quantity * product.Price;

                await _orderRepo.AddAsync(order);
            });
        }


    //My Orders
    public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
    {
        var orders = await _orderRepo.GetByUserIdAsync(userId);

        return _mapper.Map<List<OrderDto>>(orders);
    }


    //get Order by Id
    public async Task<OrderDto?> GetByIdAsync(int orderId, int userId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);

        if (order == null)
            return null;

        if (order.UserId != userId)
            return null;

        return _mapper.Map<OrderDto>(order);
    }

    //Order Cancel
    public async Task<OrderDto?> CancelOrderAsync(int orderId , int userId)
    {
        var order = await _orderRepo.GetByIdForUpdateAsync(orderId);

        if (order == null)
            return null ;

        if (order.UserId != userId)
            return null;

        if (order.Status != OrderStatus.Pending)
            throw new Exception("Only pending orders can be cancelled");

        await _orderRepo.ExecuteInTransactionAsync(async () =>
        {
            order.Status = OrderStatus.Cancelled;

            //Restore stock 

            foreach(var item in order.Items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);

                if (product != null)
                {
                    product.Stock += item.Quantity;
                }
            }
        });

        return _mapper.Map<OrderDto?>(order);
    }

    public async Task<(List<OrderDto> Orders, int TotalCount)> GetAllOrdersAsync(
    int page,
    int pageSize,
    string? status = null)
    {
        OrderStatus? parsedStatus = null;

        if (!string.IsNullOrEmpty(status) &&
       Enum.TryParse<OrderStatus>(status, true, out var result))
        {
            parsedStatus = result;
        }

        var (orders, totalCount) =
        await _orderRepo.GetAllAsync(page, pageSize, parsedStatus);

        var mapped = _mapper.Map<List<OrderDto>>(orders);

        return (mapped, totalCount);
    }


    //Update the Order Status
    public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId , string newStatus)
    {
        if (!Enum.TryParse<OrderStatus>(newStatus, true, out var parsedStatus))
            throw new Exception("Invalid order status");

        var order = await _orderRepo.GetByIdForUpdateAsync(orderId);

        if (order == null)
            return null;

        if (!IsValidTransition(order.Status, parsedStatus))
            throw new Exception(
                $"Cannot change status from {order.Status} to {parsedStatus}");

        await _orderRepo.ExecuteInTransactionAsync(async () =>
        {
            if (parsedStatus == OrderStatus.Cancelled &&
            order.Status != OrderStatus.Cancelled)
            {
                foreach(var item in order.Items)
                {
                   var product =
                   await _productRepo.GetByIdAsync(item.ProductId);

                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                    }
                }
            }

            order.Status = parsedStatus;
        });

        return _mapper.Map<OrderDto>(order);
    }


    //checking the Transaction is Valid
    private bool IsValidTransition(OrderStatus current ,OrderStatus next)
    {
        return current switch
        {
            OrderStatus.Pending =>
                next is OrderStatus.Confirmed
                or OrderStatus.Cancelled,

            OrderStatus.Confirmed =>
                next is OrderStatus.Shipped
                or OrderStatus.Cancelled,

            OrderStatus.Shipped =>
                next == OrderStatus.Delivered,

            OrderStatus.Delivered =>
                false,

            OrderStatus.Cancelled =>
                false,

            _ => false

        };
    }

    //Comfirrm Payment 
    public async Task ConfirmPaymentAsync(
    int orderId,
    int userId,
    string transactionId)
    {
        var order = await _orderRepo.GetByIdForUpdateAsync(orderId);

        if (order == null)
            throw new Exception("Order not found");

        if (order.UserId != userId)
            throw new Exception("Unauthorized access");

        if (order.PaymentMethod != PaymentMethod.Cards)
            throw new Exception("This order does not require card payment");

        if (order.PaymentStatus == PaymentStatus.Paid)
            throw new Exception("Order already paid");

        if (order.Status == OrderStatus.Cancelled)
            throw new Exception("Cannot pay for cancelled order");

        await _orderRepo.ExecuteInTransactionAsync(async () =>
        {
            order.PaymentStatus = PaymentStatus.Paid;
            order.PaymentTransactionId = transactionId;
            order.PaymentDate = DateTime.UtcNow;

            order.Status = OrderStatus.Confirmed;
        });
    }


}
