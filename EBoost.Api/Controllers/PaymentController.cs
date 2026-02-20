using EBoost.Api.Extensions;
using EBoost.Application.DTOs.Payment;
using EBoost.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EBoost.Application.DTOs.Payment;

namespace EBoost.Api.Controllers;

[Route("api/payment")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create/{orderId}")]
    public async Task<IActionResult> Create(int orderId)
    {
        var razorpayOrderId =
            await _paymentService.CreateRazorpayOrderAsync(orderId);

        return Ok(new { razorpayOrderId });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(
        [FromBody] VerifyPaymentDto dto)
    {
        var result = await _paymentService.VerifyPaymentAsync(
            dto.RazorpayOrderId,
            dto.RazorpayPaymentId,
            dto.RazorpaySignature);

        if (!result)
            return BadRequest("Payment verification failed");

        return Ok("Payment successful");
    }
}
