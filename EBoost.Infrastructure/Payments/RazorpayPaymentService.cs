using Azure.Core;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Application.Interfaces.Services;
using EBoost.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EBoost.Application.Services;

public class RazorpayPaymentService : IPaymentService
{
    private readonly IConfiguration _config;
    private readonly IOrderRepository _orderRepo;

    public RazorpayPaymentService(IConfiguration config, IOrderRepository orderRepo)
    {
        _config = config;
        _orderRepo = orderRepo;
    }

    public async Task<string> CreateRazorpayOrderAsync(int orderId)
    {
        var order = await _orderRepo.GetByIdForUpdateAsync(orderId);

        if (order == null)
            throw new Exception("Order not found");

        var key = _config["Razorpay:Key"]
            ?? throw new Exception("Razorpay key missing");

        var secret = _config["Razorpay:Secret"]
            ?? throw new Exception("Razorpay secret missing");

        var client = new RazorpayClient(key, secret);

        var options = new Dictionary<string, object>
    {
        { "amount", (int)(order.GrandTotal * 100) },
        { "currency", "INR" },
        { "receipt", order.Id.ToString() }
    };

        var razorpayOrder = client.Order.Create(options);

        order.RazorpayOrderId = razorpayOrder["id"].ToString();
        order.PaymentStatus = PaymentStatus.Pending;

        await _orderRepo.SaveChangesAsync();

        return order.RazorpayOrderId!;
    }


    private string ComputeHmacSha256(string data, string secret)
    {
        var encoding = new UTF8Encoding();
        byte[] keyBytes = encoding.GetBytes(secret);
        byte[] messageBytes = encoding.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        byte[] hash = hmac.ComputeHash(messageBytes);

        return BitConverter.ToString(hash)
            .Replace("-", "")
            .ToLower();
    }

    public async Task<bool> VerifyPaymentAsync(
    string razorpayOrderId,
    string razorpayPaymentId,
    string razorpaySignature)
    {
        var secret = _config["Razorpay:Secret"]
            ?? throw new Exception("Razorpay secret not configured");


        var generatedSignature = ComputeHmacSha256(
            razorpayOrderId + "|" + razorpayPaymentId,
            secret);

        if (generatedSignature != razorpaySignature)
            return false;

        var order = await _orderRepo
            .GetByRazorpayOrderIdAsync(razorpayOrderId);

        if (order == null)
            return false;

        order.PaymentStatus = PaymentStatus.Paid;
        order.Status = OrderStatus.Confirmed;
        order.TransactionId = razorpayPaymentId;

        await _orderRepo.SaveChangesAsync();

        return true;
    }

}
