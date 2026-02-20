using System.Net;
using System.Text.Json;
using EBoost.Application.Common.Responses;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        //catch (Exception ex)
        //{
        //    context.Response.ContentType = "application/json";
        //    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        //    var response = ApiResponse<string>.Fail(
        //        "An unexpected error occurred",
        //        StatusCodes.Status500InternalServerError
        //    );

        //    await context.Response.WriteAsJsonAsync(response);

        //}
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(
                ApiResponse<string>.Fail(
                    //ex.Message, 
                    //StatusCodes.Status500InternalServerError

                    ex.InnerException?.Message ?? ex.Message
                )
            );
        }

        //catch (Exception ex)
        //{
        //    Console.WriteLine("===== FULL ERROR =====");
        //    Console.WriteLine(ex.ToString());

        //    context.Response.StatusCode = 400;
        //    await context.Response.WriteAsJsonAsync(
        //        ApiResponse<string>.Fail(ex.Message, 400)
        //    );
        //}
    }
}
