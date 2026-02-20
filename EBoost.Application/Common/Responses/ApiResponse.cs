using Microsoft.AspNetCore.Http;

namespace EBoost.Application.Common.Responses;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    // ✅ SUCCESS RESPONSE
    public static ApiResponse<T> Ok(
        T data,
        string message = "Success",
        int statusCode = StatusCodes.Status200OK)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }

    // ✅ FAILURE RESPONSE (ONLY ONE)
    public static ApiResponse<T> Fail(
        string message,
        int statusCode = StatusCodes.Status400BadRequest)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Data = default
        };
    }

    public static ApiResponse<List<string>> FailureResponse(
    string message,
    List<string> errors,
    int statusCode = StatusCodes.Status400BadRequest)
    {
        return new ApiResponse<List<string>>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Data = errors
        };
    }

}
