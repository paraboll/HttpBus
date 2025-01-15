using Newtonsoft.Json;

namespace HttpBus.Extensions;
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _log;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _log = logger;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            _log.LogError($"Message: {ex.Message} \nStackTrace: {ex.StackTrace}");
            context.Response.StatusCode = 200;

            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                errorText = ex.Message,
                success = false
            }));
        }
    }
}
