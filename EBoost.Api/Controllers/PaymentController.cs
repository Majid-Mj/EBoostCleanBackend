using EBoost.Api.Extensions;
using EBoost.Application.Common.Responses;
using EBoost.Application.DTOs.Payment;
using EBoost.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBoost.Api.Controllers;

[Route("api/payment")]
[ApiController]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost("create/{orderId:int}")]
    public async Task<IActionResult> Create(int orderId)
    {
        try
        {
            _logger.LogInformation("[Payment] Create called for orderId={OrderId}", orderId);
            var sessionUrl = await _paymentService.CreateCheckoutSessionAsync(orderId);
            _logger.LogInformation("[Payment] Returning Stripe SessionUrl={Url} for orderId={OrderId}", sessionUrl, orderId);
            var jsonStr = $"{{\"sessionUrl\": \"{sessionUrl}\"}}";
            return Content(jsonStr, "application/json");
        }
        catch (ApplicationException ex)
        {
            _logger.LogWarning("[Payment] Business error during Create for orderId={OrderId}: {Msg}", orderId, ex.Message);
            return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Payment] Unexpected error in Create for orderId={OrderId}", orderId);
            return StatusCode(500, ApiResponse<string>.Fail("An unexpected error occurred during payment creation."));
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyPaymentDto dto)
    {
        try
        {
            var sessionId = dto?.SessionId ?? dto?.RazorpayOrderId ?? string.Empty;
            _logger.LogInformation("[Payment] Verify called for SessionId={SessionId}", sessionId);

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return BadRequest(ApiResponse<string>.Fail("Session ID is required."));
            }

            var result = await _paymentService.VerifyPaymentAsync(sessionId);

            if (!result)
            {
                _logger.LogWarning("[Payment] Payment verification failed for SessionId={SessionId}", sessionId);
                return BadRequest(ApiResponse<string>.Fail("Payment verification failed."));
            }

            _logger.LogInformation("[Payment] Payment verified successfully for SessionId={SessionId}", sessionId);
            return Ok(ApiResponse<string>.Ok("Payment successful"));
        }
        catch (ApplicationException ex)
        {
            _logger.LogWarning("[Payment] Business error during Verify: {Msg}", ex.Message);
            return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Payment] Unexpected error in Verify for SessionId={SessionId}", dto?.SessionId ?? dto?.RazorpayOrderId);
            return StatusCode(500, ApiResponse<string>.Fail("An unexpected error occurred during payment verification."));
        }
    }
}
