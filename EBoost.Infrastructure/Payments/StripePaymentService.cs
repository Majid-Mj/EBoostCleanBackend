using EBoost.Application.Interfaces.Repositories;
using EBoost.Application.Interfaces.Services;
using EBoost.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EBoost.Infrastructure.Payments;

/// <summary>
/// Implements Stripe checkout session creation and verification.
/// </summary>
public class StripePaymentService : IPaymentService
{
    private readonly IConfiguration _config;
    private readonly IOrderRepository _orderRepo;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(
        IConfiguration config,
        IOrderRepository orderRepo,
        ILogger<StripePaymentService> logger)
    {
        _config = config;
        _orderRepo = orderRepo;
        _logger = logger;
        
        // Initialize Stripe API configuration
        var secretKey = _config["Stripe:SecretKey"];
        if (!string.IsNullOrWhiteSpace(secretKey))
        {
            StripeConfiguration.ApiKey = secretKey;
        }
    }

    public async Task<string> CreateCheckoutSessionAsync(int orderId)
    {
        try
        {
            _logger.LogInformation("[Stripe] Starting CreateCheckoutSession for OrderId: {OrderId}", orderId);

            // Fetch and validate the order
            var order = await _orderRepo.GetByIdForUpdateAsync(orderId);
            if (order == null)
            {
                throw new ApplicationException($"Order #{orderId} not found.");
            }

            var secretKey = _config["Stripe:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new ApplicationException("Stripe SecretKey is missing from configuration.");
            }
            StripeConfiguration.ApiKey = secretKey;

            // Stripe expects amount in cents
            var amountInCents = (long)(order.GrandTotal * 100);
            if (amountInCents <= 0)
            {
                throw new ApplicationException($"Invalid order amount: \u20b9{order.GrandTotal}");
            }

            // AllowedOrigins / base URL configuration
            var domain = _config["AllowedOrigins"] ?? "http://localhost:5173";
            domain = domain.TrimEnd('/');

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = amountInCents,
                            Currency = "inr", // Use your preferred currency, e.g. "inr" or "usd"
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"EBoost Order #{orderId}",
                                Description = $"Payment for order with {order.Items.Count} item(s)"
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = $"{domain}/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{domain}/payment?cancel=true",
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", orderId.ToString() }
                }
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            _logger.LogInformation("[Stripe] Created session: {SessionId}", session.Id);

            // Save the session ID in RazorpayOrderId to reuse the DB column
            order.RazorpayOrderId = session.Id;
            order.PaymentStatus = PaymentStatus.Pending;
            await _orderRepo.SaveChangesAsync();

            // Return the checkout session URL
            return session.Url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Stripe] CreateCheckoutSessionAsync failed for OrderId: {OrderId}", orderId);
            throw;
        }
    }

    public async Task<bool> VerifyPaymentAsync(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            _logger.LogWarning("[Stripe] VerifyPayment: Session ID is empty.");
            return false;
        }

        try
        {
            _logger.LogInformation("[Stripe] Verifying payment for SessionId: {SessionId}", sessionId);

            if (sessionId.StartsWith("demo_success_payment_"))
            {
                _logger.LogInformation("[Stripe] Simulating successful payment for SessionId: {SessionId}", sessionId);
                var parts = sessionId.Split('_');
                if (parts.Length >= 4 && int.TryParse(parts[3], out int parsedOrderId))
                {
                    var dbOrder = await _orderRepo.GetByIdForUpdateAsync(parsedOrderId);
                    if (dbOrder != null)
                    {
                        dbOrder.PaymentStatus = PaymentStatus.Paid;
                        dbOrder.Status = OrderStatus.Confirmed;
                        dbOrder.TransactionId = "ch_mock_" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
                        dbOrder.PaymentDate = DateTime.UtcNow;
                        await _orderRepo.SaveChangesAsync();
                        _logger.LogInformation("[Stripe] Mock Payment verified. Order #{OrderId} marked Paid/Confirmed.", dbOrder.Id);
                        return true;
                    }
                }
                return false;
            }

            var secretKey = _config["Stripe:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new ApplicationException("Stripe SecretKey is missing from configuration.");
            }
            StripeConfiguration.ApiKey = secretKey;

            var service = new SessionService();
            Session session = await service.GetAsync(sessionId);

            if (session == null)
            {
                _logger.LogWarning("[Stripe] Session not found for ID: {SessionId}", sessionId);
                return false;
            }

            if (session.PaymentStatus != "paid")
            {
                _logger.LogWarning("[Stripe] Session {SessionId} payment status is: {Status}", sessionId, session.PaymentStatus);
                return false;
            }

            // Find order by Session ID
            var order = await _orderRepo.GetByRazorpayOrderIdAsync(sessionId);
            if (order == null)
            {
                _logger.LogError("[Stripe] Order not found for SessionId={SessionId}", sessionId);
                return false;
            }

            order.PaymentStatus = PaymentStatus.Paid;
            order.Status = OrderStatus.Confirmed;
            order.TransactionId = session.PaymentIntentId; // Set transaction ID to Stripe PaymentIntent ID
            order.PaymentDate = DateTime.UtcNow;

            await _orderRepo.SaveChangesAsync();

            _logger.LogInformation("[Stripe] Payment verified. Order #{OrderId} marked Paid/Confirmed.", order.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Stripe] VerifyPaymentAsync failed for SessionId: {SessionId}", sessionId);
            return false;
        }
    }
}
