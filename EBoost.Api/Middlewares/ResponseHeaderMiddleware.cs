public class ResponseHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-App-Name"] = "EBoost";
            context.Response.Headers["X-App-Version"] = "1.0";

            // Remove server header (best-effort)
            context.Response.Headers.Remove("Server");

            return Task.CompletedTask;
        });

        await _next(context);
    }
}
