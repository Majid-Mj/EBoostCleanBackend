using EBoost.Application.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ApiResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null) return;

        if (context.Result is ObjectResult objectResult)
        {
            var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
            var isSuccess = statusCode >= 200 && statusCode < 300;

            // Prevent double wrapping
            if (objectResult.Value?.GetType().IsGenericType == true &&
                objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                return;
            }

            string message;
            object? data = null;

            if (objectResult.Value is string str)
            {
                message = str;
            }
            else
            {
                message = isSuccess ? "Success" : "Failed";
                data = objectResult.Value;
            }

            var response = new ApiResponse<object?>
            {
                IsSuccess = isSuccess,
                StatusCode = statusCode,
                Message = message,
                Data = isSuccess ? data : null
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
        }
    }
}
