using EBoost.Application.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ApiResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        try
        {
            if (context.Exception != null) return;

            Console.WriteLine("[ApiResponseFilter] OnActionExecuted START");

            if (context.Result is ObjectResult objectResult)
            {
                Console.WriteLine("[ApiResponseFilter] Result is ObjectResult");
                var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
                var isSuccess = statusCode >= 200 && statusCode < 300;

                // Prevent double wrapping
                if (objectResult.Value != null)
                {
                    Console.WriteLine($"[ApiResponseFilter] Value Type: {objectResult.Value.GetType().FullName}");
                    if (objectResult.Value.GetType().IsGenericType)
                    {
                        Console.WriteLine($"[ApiResponseFilter] Value is GenericType: {objectResult.Value.GetType().GetGenericTypeDefinition().FullName}");
                        if (objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>))
                        {
                            Console.WriteLine("[ApiResponseFilter] Double wrapping prevented.");
                            return;
                        }
                    }
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

                Console.WriteLine("[ApiResponseFilter] Setting new ObjectResult...");
                context.Result = new ObjectResult(response)
                {
                    StatusCode = statusCode
                };
                Console.WriteLine("[ApiResponseFilter] OnActionExecuted END");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ApiResponseFilter] FATAL CRASH: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }
}
