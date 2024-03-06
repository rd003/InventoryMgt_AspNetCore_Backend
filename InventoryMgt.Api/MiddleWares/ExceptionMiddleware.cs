
using InventoryMgt.Api.CustomExceptions;
using InventoryMgt.Api.Models;

namespace InventoryMgt.Api.Middlewares;
public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            await HandleException(context, ex);
        }
    }

    private static Task HandleException(HttpContext context, Exception ex)
    {
        int statusCode = StatusCodes.Status500InternalServerError;
        switch (ex)
        {
            case NotFoundException _:
                statusCode = StatusCodes.Status404NotFound;
                break;
            case BadRequestException _:
                statusCode = StatusCodes.Status400BadRequest;
                break;
        }
        var errorResponse = new ErrorResponse
        {
            Message = ex.Message,
            StatusCode = statusCode
        };
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(errorResponse.ToString());
    }

}

public static class ExceptionMiddlewareExtension
{
    public static void ConfigureExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}