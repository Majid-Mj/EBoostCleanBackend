using EBoost.Application.Common.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult Success<T>(
        T data,
        string message = "Success",
        int statusCode = StatusCodes.Status200OK)
    {
        return StatusCode(
            statusCode,
            ApiResponse<T>.Ok(data, message, statusCode)
        );
    }

    protected IActionResult Failure(
        string message,
        int statusCode = StatusCodes.Status400BadRequest)
    {
        return StatusCode(
            statusCode,
            ApiResponse<string>.Fail(message, statusCode)
        );
    }


}
